using HauteCouture.Example.Contracts.Constants;
using HauteCouture.Shared.WebApi.Endpoints.Abstractions;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.DeleteSomething;

/// <summary>
///     Maps the endpoint for soft-deleting an existing <c>Something</c>.
/// </summary>
public sealed class DeleteSomethingEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(Routes.DeleteSomething, DeleteSomethingHandler.Handle)
            .WithName(EndpointName.DeleteSomething)
            .WithTags(EndpointTag.Something)
            .WithSummary("Deletes a Something.")
            .WithDescription("Soft-deletes the Something with the specified identifier.")
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}