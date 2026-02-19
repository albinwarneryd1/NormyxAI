namespace Sylvaro.Application.Abstractions;

public interface IAssessmentExecutionGuard
{
    ValueTask<IAsyncDisposable> AcquireAsync(Guid tenantId, Guid versionId, CancellationToken cancellationToken = default);
}
