using HauteCouture.Shared.Databases.Postgres.Migrations.Registration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HauteCouture.Example.HostSide.Private.Migrations.Registration;

public static class MigrationHostRegistrar
{
    private const string ConnectionStringName = "Postgres";

    public static IServiceCollection AddMigrationHost(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly().FullName;

        services
            .AddDbContext<SomethingDbContext>(options =>
            {
                options
                    .UseNpgsql(
                        GetConnectionString(configuration),
                            optionsBuilder => optionsBuilder.MigrationsAssembly(assembly))
                    .UseSnakeCaseNamingConvention();
            })
            .AddMigrationExecutor<SomethingDbContext>()
            .AddSingleton<MigrationStartup>();

        return services;
    }

    private static string GetConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString(ConnectionStringName) 
            ?? throw new InvalidOperationException($"Connection string '{ConnectionStringName}' was not found.");
    }
}