using System.Text.Json;
using Normyx.Application.Abstractions;
using Normyx.Application.Compliance;

namespace Normyx.Infrastructure.Compliance;

public class AiDraftService : IAiDraftService
{
    public Task<DraftActionPlanJson> GenerateActionPlanAsync(AssessmentSummary summary, IReadOnlyCollection<FindingDraft> findings, CancellationToken cancellationToken = default)
    {
        var actions = findings
            .Select((finding, index) => new DraftActionItem(
                $"Address: {finding.Title}",
                finding.Description,
                finding.Severity switch
                {
                    Normyx.Domain.Enums.FindingSeverity.Critical => "P0",
                    Normyx.Domain.Enums.FindingSeverity.High => "P1",
                    Normyx.Domain.Enums.FindingSeverity.Medium => "P2",
                    _ => "P3"
                },
                index % 2 == 0 ? "SecurityLead" : "ComplianceOfficer",
                $"Control objective met and evidence uploaded for {finding.Title}.",
                finding.EvidenceSuggestions.Length == 0 ? ["Policy evidence", "Control evidence"] : finding.EvidenceSuggestions))
            .ToArray();

        var json = new DraftActionPlanJson(actions);
        ValidateActionPlan(json);

        return Task.FromResult(json);
    }

    public Task<DraftDpiaJson> GenerateDpiaDraftAsync(AssessmentSummary summary, IReadOnlyCollection<FindingDraft> findings, CancellationToken cancellationToken = default)
    {
        var sections = new List<DraftDpiaSection>
        {
            new("Processing context", ["AI system assessed as " + summary.AiActRiskClass, "GDPR flags: " + string.Join(", ", summary.GdprFlags)], ["Confirm purpose limitation wording"]),
            new("Risks", findings.Select(x => x.Title).Take(8).ToArray(), ["Legal review required for high-risk findings"]),
            new("Mitigations", ["Action plan approved", "Evidence links attached"], ["Complete supplier transfer assessment"])
        };

        var json = new DraftDpiaJson(sections);
        ValidateDpia(json);

        return Task.FromResult(json);
    }

    private static void ValidateActionPlan(DraftActionPlanJson json)
    {
        if (json.Actions.Count == 0)
        {
            throw new InvalidOperationException("AI draft validation failed: actions[] required");
        }

        foreach (var action in json.Actions)
        {
            if (string.IsNullOrWhiteSpace(action.Priority) || string.IsNullOrWhiteSpace(action.OwnerRole) || string.IsNullOrWhiteSpace(action.AcceptanceCriteria))
            {
                throw new InvalidOperationException("AI draft validation failed: priority/ownerRole/acceptanceCriteria required");
            }
        }

        _ = JsonSerializer.Serialize(json);
    }

    private static void ValidateDpia(DraftDpiaJson json)
    {
        if (json.Sections.Count == 0 || json.Sections.Any(x => string.IsNullOrWhiteSpace(x.Title)))
        {
            throw new InvalidOperationException("AI DPIA validation failed: sections[] with title required");
        }

        _ = JsonSerializer.Serialize(json);
    }
}
