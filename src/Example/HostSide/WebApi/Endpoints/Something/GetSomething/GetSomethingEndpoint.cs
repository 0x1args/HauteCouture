using HauteCouture.Example.Contracts.Constants;
using HauteCouture.Example.Contracts.Responses;
using HauteCouture.Shared.WebApi.Endpoints.Abstractions;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.GetSomething;

/// <summary>
///     Maps the endpoint for retrieving a single <c>Something</c> by its identifier.
/// </summary>
public sealed class GetSomethingEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Routes.GetSomething, GetSomethingHandler.Handle)
            .WithName(EndpointName.GetSomething)
            .WithTags(EndpointTag.Something)
            .WithSummary("Gets a Something.")
            .WithDescription("Retrieves the Something with the specified identifier.")
            .Produces<SomethingResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}