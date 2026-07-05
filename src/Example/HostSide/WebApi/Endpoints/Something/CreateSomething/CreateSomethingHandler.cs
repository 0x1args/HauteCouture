using HauteCouture.Example.Applications.Handlers.Commands.CreateSomething;
using HauteCouture.Example.Contracts.Requests;
using HauteCouture.Shared.Common.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.CreateSomething;

/// <summary>
///     Handles HTTP requests for <see cref="CreateSomethingEndpoint"/>.
/// </summary>
public class CreateSomethingHandler
{
    /// <summary>
    ///     Creates a new <c>Something</c> owned by the current user.
    /// </summary>
    public static async Task<IResult> Handle(
        [FromBody] CreateSomethingRequest request,
        [FromServices] ICurrentUserSession currentUser,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateSomethingCommand(request, currentUser.UserId!.Value);
        var somethingId = await sender.Send(command, cancellationToken);

        return Results.Ok(somethingId);
    }
}