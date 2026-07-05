using HauteCouture.Example.Contracts.Constants;
using HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.CreateSomething;
using HauteCouture.Shared.WebApi.Endpoints.Abstractions;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.UpdateSomethingDescription;

/// <summary>
///     Maps the endpoint for updating the description of an existing <c>Something</c>.
/// </summary>
public class UpdateSomethingDescriptionEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(Routes.UpdateSomethingDescription, UpdateSomethingDescriptionHandler.Handle)
            .WithName(EndpointName.UpdateSomethingDescription)
            .WithTags(EndpointTag.Something)
            .WithSummary("Updates a Something's description.")
            .WithDescription("Updates the description of the Something with the specified identifier.")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}