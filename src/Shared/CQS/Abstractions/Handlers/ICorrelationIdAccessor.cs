namespace HauteCouture.Shared.CQS.Abstractions.Handlers;

/// <summary>
///     Provides access to the correlation ID for the current HTTP request scope.
/// </summary>
public interface ICorrelationIdAccessor
{
    /// <summary>
    ///     Correlation ID associated with the current request.
    /// </summary>
    Guid CorrelationId { get; }
}