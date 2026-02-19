using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Normyx.Api.Utilities;
using Normyx.Application.Abstractions;
using Normyx.Application.Security;
using Normyx.Domain.Entities;
using Normyx.Infrastructure.Persistence;

namespace Normyx.Api.Endpoints;

public static class ExportEndpoints
{
    public static IEndpointRouteBuilder MapExportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/exports").WithTags("Exports").RequireAuthorization();

        group.MapPost("/versions/{versionId:guid}/generate", GenerateExportAsync)
            .RequireAuthorization(new AuthorizeAttribute { Roles = $"{RoleNames.Admin},{RoleNames.ComplianceOfficer}" });
        group.MapGet("/{artifactId:guid}/download", DownloadExportAsync);

        return app;
    }

    public record GenerateExportRequest(string ExportType);

    private static async Task<IResult> GenerateExportAsync(
        [FromRoute] Guid versionId,
        [FromBody] GenerateExportRequest request,
        NormyxDbContext dbContext,
        ICurrentUserContext currentUser,
        IExportService exportService,
        IObjectStorage objectStorage)
    {
        var tenantId = TenantContext.RequireTenantId(currentUser);
        var userId = TenantContext.RequireUserId(currentUser);

        var version = await dbContext.AiSystemVersions
            .Include(x => x.AiSystem)
            .FirstOrDefaultAsync(x => x.Id == versionId && x.AiSystem.TenantId == tenantId);

        if (version is null)
        {
            return Results.NotFound();
        }

        var latestAssessment = await dbContext.Assessments
            .Where(x => x.AiSystemVersionId == versionId)
            .OrderByDescending(x => x.RanAt)
            .FirstOrDefaultAsync();

        var actions = await dbContext.ActionItems
            .Where(x => x.AiSystemVersionId == versionId)
            .OrderBy(x => x.Priority)
            .ToListAsync();

        var lines = new List<string>
        {
            $"Export Type: {request.ExportType}",
            $"AI System: {version.AiSystem.Name}",
            $"Version: {version.VersionNumber}",
            $"Generated: {DateTimeOffset.UtcNow:O}",
            ""
        };

        if (latestAssessment is not null)
        {
            lines.Add("Latest assessment scores:");
            lines.Add(latestAssessment.RiskScoresJson);
            lines.Add("");
        }

        lines.Add("Action plan:");
        lines.AddRange(actions.Select(x => $"- [{x.Priority}] {x.Title} ({x.Status}) owner={x.OwnerRole}"));

        var pdfBytes = await exportService.GeneratePdfAsync($"Normyx AI {request.ExportType}", lines);

        await using var stream = new MemoryStream(pdfBytes);
        var storageRef = await objectStorage.SaveAsync($"{request.ExportType}-{version.AiSystem.Name}-v{version.VersionNumber}.pdf", "application/pdf", stream);

        var artifact = new ExportArtifact
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            AiSystemVersionId = versionId,
            ExportType = request.ExportType,
            StorageRef = storageRef,
            CreatedByUserId = userId,
            CreatedAt = DateTimeOffset.UtcNow
        };

        dbContext.ExportArtifacts.Add(artifact);
        await dbContext.SaveChangesAsync();

        return Results.Ok(new { artifact.Id, artifact.ExportType, artifact.CreatedAt });
    }

    private static async Task<IResult> DownloadExportAsync([FromRoute] Guid artifactId, NormyxDbContext dbContext, ICurrentUserContext currentUser, IObjectStorage objectStorage)
    {
        var tenantId = TenantContext.RequireTenantId(currentUser);
        var artifact = await dbContext.ExportArtifacts.FirstOrDefaultAsync(x => x.Id == artifactId && x.TenantId == tenantId);

        if (artifact is null)
        {
            return Results.NotFound();
        }

        var (stream, contentType) = await objectStorage.OpenReadAsync(artifact.StorageRef);
        var fileName = $"{artifact.ExportType}-{artifact.Id}.pdf";
        return Results.File(stream, contentType, fileName);
    }
}
