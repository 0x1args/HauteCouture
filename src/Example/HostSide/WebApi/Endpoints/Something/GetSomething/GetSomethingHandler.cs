using HauteCouture.Example.Applications.Handlers.Queries.GetSomething;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.GetSomething;

/// <summary>
///     Handles HTTP requests for <see cref="GetSomethingEndpoint"/>.
/// </summary>
public class GetSomethingHandler
{
    /// <summary>
    ///     Retrieves a single <c>Something</c> by its identifier.
    /// </summary>
    public static async Task<IResult> Handle(
        [FromRoute] Guid somethingId,
        [FromServices] ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new GetSomethingQuery(somethingId);
        var response = await sender.Send(command, cancellationToken);

        return Results.Ok(response);
    }
}