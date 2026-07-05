using MediatR;
using System.Diagnostics;

namespace HauteCouture.Shared.CQS.Abstractions.Handlers;

/// <summary>
///     Base diagnostic handler that provides helper methods for tag/baggage enrichment.
/// </summary>
/// <typeparam name="TRequest">Type of the request.</typeparam>
/// <typeparam name="TResponse">Type of the response.</typeparam>
public abstract class BaseDiagnosticHandler<TRequest, TResponse>
    : IDiagnosticHandler<TRequest, TResponse>
    where TRequest : IBaseRequest
{
    /// <inheritdoc/>
    public abstract void Before(TRequest request, Activity? activity);

    /// <inheritdoc/>
    public abstract void After(Activity? activity, TRequest request, TResponse response);

    /// <summary>
    ///     Sets a tag (span attribute) on the activity if it is not <c>null</c>.
    /// </summary>
    protected static void SetTag(Activity? activity, string key, object? value)
        => activity?.SetTag(key, value);

    /// <summary>
    ///     Adds a distributed baggage entry propagated to downstream services.
    /// </summary>
    protected static void SetBaggage(Activity? activity, string key, string? value)
        => activity?.SetBaggage(key, value);

    /// <summary>
    ///     Records a structured event (annotation) on the span timeline.
    /// </summary>
    protected static void AddEvent(Activity? activity, string name, ActivityTagsCollection? tags = null)
        => activity?.AddEvent(new ActivityEvent(name, tags: tags ?? []));
}