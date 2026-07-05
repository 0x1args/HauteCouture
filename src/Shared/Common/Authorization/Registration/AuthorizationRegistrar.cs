using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Shared.Common.Authorization.Registration;

/// <summary>
///     Registrar for authorization-related services.
/// </summary>
public static class AuthorizationRegistrar
{
    /// <summary>
    ///     Registers services related to user session management and authorization context.
    /// </summary>
    /// <param name="services">DI service collection.</param>
    /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddUserSessions(
        this IServiceCollection services,
        Func<IServiceProvider, ICurrentUserSession> factory)
    {
        services.AddScoped(factory);
        return services;
    }
}