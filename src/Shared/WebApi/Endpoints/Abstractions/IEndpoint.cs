using Microsoft.AspNetCore.Routing;

namespace HauteCouture.Shared.WebApi.Endpoints.Abstractions;

/// <summary>
///     Contract for defining http endpoints in the application.
/// </summary>
public interface IEndpoint
{
    /// <summary>
    ///     Registers an http route for the current endpoint.
    /// </summary>
    /// <param name="app">Routing provider used to register endpoints.</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}