using HauteCouture.Shared.WebApi.Endpoints.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace HauteCouture.Shared.WebApi.Endpoints;

/// <summary>
///     Extension for automatic registration and mapping of http endpoints.
/// </summary>
public static class EndpointRegistrar
{
    /// <summary>
    ///     Registers all implementations of <see cref="IEndpoint"/> as transient services.
    /// </summary>
    /// <param name="services">Service collection. </param>
    /// <param name="assembly">Assembly in which <see cref="IEndpoint"/> implementations are searched for. </param>
    /// <returns>Modified <see cref="IServiceCollection"/>. </returns>
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var endpointTypes = assembly.GetTypes()
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type));

        services.TryAddEnumerable(endpointTypes);

        return services;
    }

    /// <summary>
    ///     Registers all http endpoints defined by implementations of <see cref="IEndpoint"/>.
    /// </summary>
    /// <param name="app">Web application to register endpoints with. </param>
    /// <param name="route">Route group builder to register endpoints with. </param>
    /// <returns>Modified <see cref="WebApplication"/>. </returns>
    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? route = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder = route is not null
            ? route
            : app;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}