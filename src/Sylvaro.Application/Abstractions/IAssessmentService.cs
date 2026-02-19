using Sylvaro.Application.Compliance;

namespace Sylvaro.Application.Abstractions;

public interface IAssessmentService
{
    Task<AssessmentRunResult> RunAssessmentAsync(Guid tenantId, Guid versionId, Guid ranByUserId, CancellationToken cancellationToken = default);
}
