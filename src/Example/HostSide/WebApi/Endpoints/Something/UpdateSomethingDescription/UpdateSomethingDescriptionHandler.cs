using HauteCouture.Example.Applications.Handlers.Commands.UpdateSomethingDescription;
using HauteCouture.Example.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.UpdateSomethingDescription;

/// <summary>
///     Handles HTTP requests for <see cref="UpdateSomethingDescriptionEndpoint"/>.
/// </summary>
public class UpdateSomethingDescriptionHandler
{
    /// <summary>
    ///     Updates the description of an existing <c>Something</c>.
    /// </summary>
    public static async Task<IResult> Handle(
        [FromRoute] Guid somethingId,
        [FromBody] UpdateSomethingDescriptionRequest request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateSomethingDescriptionCommand(somethingId, request);
        await sender.Send(command, cancellationToken);

        return Results.Ok();
    }
}