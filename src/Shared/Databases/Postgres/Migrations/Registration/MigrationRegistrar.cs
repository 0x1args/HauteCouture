using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HauteCouture.Shared.Databases.Postgres.Migrations.Registration;

/// <summary>
///     Registrar for migration-related services.
/// </summary>
public static class MigrationRegistrar
{
    /// <summary>
    ///     Registers a migration executor for the specified <see cref="DbContext"/> type.
    /// </summary>
    /// <param name="services">DI service collection.</param>
    /// <returns>Modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMigrationExecutor<TDbContext>(
        this IServiceCollection services) 
        where TDbContext : DbContext
    {
        services.AddSingleton<IMigrationExecutor, MigrationExecutor<TDbContext>>();
        return services;
    }
}