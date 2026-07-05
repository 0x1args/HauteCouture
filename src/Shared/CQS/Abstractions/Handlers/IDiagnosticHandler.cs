using MediatR;
using System.Diagnostics;

namespace HauteCouture.Shared.CQS.Abstractions.Handlers;

/// <summary>
///     Diagnostic handler for tracing message processing via <see cref="Activity"/>.
/// </summary>
/// <typeparam name="TRequest">Type of request.</typeparam>
public interface IDiagnosticHandler<in TRequest>
    where TRequest : IBaseRequest
{
    /// <summary>
    ///     Starts and enriches a new <see cref="Activity"/> before request processing.
    /// </summary>
    /// <param name="request">Incoming request.</param>
    /// <param name="activity">Active span (may be <c>null</c> if sampling dropped it).</param>
    void Before(TRequest request, Activity? activity);

    /// <summary>
    ///     Stops the activity after request processing completes.
    /// </summary>
    /// <param name="activity">Activity to stop.</param>
    /// <param name="request">Request that was processed.</param>
    void After(
        Activity? activity, 
        TRequest request);
}

/// <summary>
///     Diagnostic handler for tracing request processing via <see cref="Activity"/> with a result.
/// </summary>
/// <typeparam name="TRequest">Type of request.</typeparam>
/// <typeparam name="TResponse">Type of request result.</typeparam>
public interface IDiagnosticHandler<in TRequest, in TResponse>
    where TRequest : IBaseRequest
{
    /// <summary>
    ///     Starts and enriches a new <see cref="Activity"/> before request processing.
    /// </summary>
    /// <param name="request">Incoming request.</param>
    /// <param name="activity">Active span (may be <c>null</c> if sampling dropped it).</param>
    void Before(TRequest request, Activity? activity);

    /// <summary>
    ///     Stops the activity after request processing completes.
    /// </summary>
    /// <param name="activity">Activity to stop.</param>
    /// <param name="request">Request that was processed.</param>
    /// <param name="response">Result returned by the request handler.</param>
    void After(
        Activity? activity,
        TRequest request, 
        TResponse response);
}