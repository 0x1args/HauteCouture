using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Shared.WebApi.Modules.Correlation;

/// <summary>
///     Scoped implementation of <see cref="ICorrelationIdAccessor"/>.
///     Populated by <see cref="CorrelationIdMiddleware"/> early in the pipeline.
/// </summary>
public sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
    /// <inheritdoc />
    public Guid CorrelationId { get; internal set; }
}