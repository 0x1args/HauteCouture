using HauteCouture.Shared.WebApi.Modules.Caching;
using HauteCouture.Shared.Common.Authorization.Registration;
using System.Reflection;
using HauteCouture.Shared.Common.Authorization;
using HauteCouture.Shared.WebApi.Registration;
using HauteCouture.Shared.WebApi.Modules.HealthCheck.Contributors.Postgres;
using HauteCouture.Shared.WebApi.Modules.HealthCheck.Contributors.Redis;
using HauteCouture.Shared.WebApi.Endpoints;

namespace HauteCouture.TenantManagement.HostSide.WebApi.Registration;

/// <summary>
///     Provides extension methods for registering the TenantManagement service's.
/// </summary>
public static class WebApiHostRegistrar
{
    private const string PostgresConnectionStringName = "Postgres";

    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers all services required by the TenantManagement web API host.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="environment">Web hosting environment.</param>
        /// <param name="host">Application host builder.</param>
        /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
        public IServiceCollection AddTenantManagementWebApi(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IHostBuilder host)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var (redisConnectionString, redisPassword) = GetRedisOptions(configuration);

            services
                .AddOpenApi()
                .AddHttpContextAccessor()
                .AddUserSessions(_ => new CurrentUserSession(
                    userId: Guid.NewGuid(),
                    tenantId: Guid.NewGuid(),
                    roles:
                    [ 
                        // Mok version.
                        UserRole.PlatformAdministrator
                    ],
                    sessionId: Guid.NewGuid(),
                    ipAddress: "127.0.0.1",
                    userAgent: "Test User Agent"))
                .AddWebModules(configuration, environment, host, options => options
                    .FromAssemblies(assembly)
                    .UseCaching()
                    .UseLogging()
                    .UseCorrelation()
                    .UseExceptionHandling()
                    .UseTrafficControl()
                    .UseHealthChecks(health => health
                        .UsePostgres(new PostgresHealthCheckOptions
                        {
                            ConnectionString = GetPostgresConnectionString(configuration)
                        })
                        .UseRedis(new RedisHealthCheckOptions
                        {
                            ConnectionString = redisConnectionString,
                            Password = redisPassword
                        }))
                    .WithModules())
                .AddEndpoints(assembly);

            return services;
        }
    }

    private static string GetPostgresConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString(PostgresConnectionStringName)
            ?? throw new InvalidOperationException($"Connection string '{PostgresConnectionStringName}' was not found.");
    }

    private static (string ConnectionString, string Password) GetRedisOptions(IConfiguration configuration)
    {
        var options = configuration
            .GetSection(CachingOptions.SectionName)
            .Get<CachingOptions>()
                ?? throw new InvalidOperationException(
                $"Missing required configuration section '{CachingOptions.SectionName}'.");

        return (options.ConnectionString, options.Password);
    }
}