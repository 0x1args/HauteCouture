using MediatR;
using HauteCouture.Shared.Common.Exceptions.Client;

namespace HauteCouture.Shared.CQS.Abstractions.Handlers;

/// <summary>
///     Authorization handler that validates access before message processing.
/// </summary>
/// <typeparam name="TRequest">Type of request.</typeparam>
public interface IAuthorizationHandler<in TRequest>
    where TRequest : IBaseRequest
{
    /// <summary>
    ///     Determines whether the current user is authorized to process the request.
    /// </summary>
    /// <param name="request">Request to authorize.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <exception cref="UnauthorizedException">
    ///     Thrown when the current user does not have access to process the request.
    /// </exception>
    Task AuthorizeAsync(
        TRequest request,
        CancellationToken cancellationToken);
}