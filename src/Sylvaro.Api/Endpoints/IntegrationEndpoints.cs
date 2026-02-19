using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sylvaro.Api.Utilities;
using Sylvaro.Application.Abstractions;
using Sylvaro.Application.Security;
using Sylvaro.Domain.Entities;
using Sylvaro.Infrastructure.Persistence;

namespace Sylvaro.Api.Endpoints;

public static class IntegrationEndpoints
{
    public static IEndpointRouteBuilder MapIntegrationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/integrations").WithTags("Integrations").RequireAuthorization(
            new AuthorizeAttribute { Roles = RoleNames.Admin }).WithRequestValidation();

        group.MapGet("/webhooks", ListWebhooksAsync);
        group.MapPut("/webhooks/{provider}", UpsertWebhookAsync);
        group.MapPost("/webhooks/{provider}/test", TestWebhookAsync);

        return app;
    }

    private static async Task<IResult> ListWebhooksAsync(SylvaroDbContext dbContext, ICurrentUserContext currentUser)
    {
        var tenantId = TenantContext.RequireTenantId(currentUser);

        var integrations = await dbContext.TenantIntegrations
            .Where(x => x.TenantId == tenantId)
            .OrderBy(x => x.Provider)
            .Select(x => new
            {
                x.Id,
                x.Provider,
                x.WebhookUrl,
                x.IsEnabled,
                x.CreatedAt,
                x.UpdatedAt
            })
            .ToListAsync();

        return Results.Ok(integrations);
    }

    private record UpsertWebhookRequest(
        [property: Required, Url, StringLength(1000)] string WebhookUrl,
        [property: StringLength(2000)] string? AuthHeader,
        bool IsEnabled);

    private static async Task<IResult> UpsertWebhookAsync(
        [FromRoute] string provider,
        [FromBody] UpsertWebhookRequest request,
        SylvaroDbContext dbContext,
        ICurrentUserContext currentUser)
    {
        var tenantId = TenantContext.RequireTenantId(currentUser);

        var existing = await dbContext.TenantIntegrations
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Provider == provider);

        if (existing is null)
        {
            existing = new TenantIntegration
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Provider = provider,
                CreatedAt = DateTimeOffset.UtcNow
            };

            dbContext.TenantIntegrations.Add(existing);
        }

        existing.WebhookUrl = request.WebhookUrl;
        existing.AuthHeader = request.AuthHeader;
        existing.IsEnabled = request.IsEnabled;
        existing.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> TestWebhookAsync(
        [FromRoute] string provider,
        SylvaroDbContext dbContext,
        ICurrentUserContext currentUser,
        IWebhookPublisher webhookPublisher)
    {
        var tenantId = TenantContext.RequireTenantId(currentUser);

        var integration = await dbContext.TenantIntegrations
            .FirstOrDefaultAsync(x => x.TenantId == tenantId && x.Provider == provider && x.IsEnabled);

        if (integration is null)
        {
            return Results.NotFound(new { message = "Enabled webhook integration not found." });
        }

        var payload = new
        {
            eventType = "sylvaro.webhook.test",
            timestamp = DateTimeOffset.UtcNow,
            tenantId,
            provider,
            message = "Sylvaro webhook test event"
        };

        var result = await webhookPublisher.PublishAsync(integration.WebhookUrl, integration.AuthHeader, payload);
        return Results.Ok(result);
    }
}
