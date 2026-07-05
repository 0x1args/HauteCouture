using HauteCouture.Example.Applications.Handlers.Commands.DeleteSomething;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.DeleteSomething;

/// <summary>
///     Handles HTTP requests for <see cref="DeleteSomethingEndpoint"/>.
/// </summary>
public class DeleteSomethingHandler
{
    /// <summary>
    ///     Soft-deletes an existing <c>Something</c>.
    /// </summary>
    public static async Task<IResult> Handle(
        [FromRoute] Guid somethingId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSomethingCommand(somethingId);
        await sender.Send(command, cancellationToken);

        return Results.Ok(somethingId);
    }
}