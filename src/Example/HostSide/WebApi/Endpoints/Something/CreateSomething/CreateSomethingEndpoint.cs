using HauteCouture.Example.Contracts.Constants;
using HauteCouture.Shared.WebApi.Endpoints.Abstractions;

namespace HauteCouture.Example.HostSide.Public.WebApi.Endpoints.Something.CreateSomething;

/// <summary>
///     Maps the endpoint for creating a new <c>Something</c>.
/// </summary>
public sealed class CreateSomethingEndpoint : IEndpoint
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Routes.CreateSomething, CreateSomethingHandler.Handle)
            .WithName(EndpointName.CreateSomething)
            .WithTags(EndpointTag.Something)
            .WithSummary("Creates a new Something.")
            .WithDescription("Creates a new Something with the specified name, description, and price, owned by the current user.")
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);
    }
}