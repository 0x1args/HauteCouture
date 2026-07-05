using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HauteCouture.Shared.Databases.Postgres.Migrations;

/// <summary>
///     Executor for database migrations.
///     Default implementation of <see cref="IMigrationExecutor"/>.
/// </summary>
/// <typeparam name="TDbContext">Type of <see cref="DbContext"/>. </typeparam>
public sealed class MigrationExecutor<TDbContext>(
    IServiceProvider provider)
    : IMigrationExecutor
    where TDbContext : DbContext
{
    /// <inheritdoc/>
    public async Task MigrateAsync(
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var name = typeof(TDbContext).Name;

        using var scope = provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        try
        {
            logger.LogInformation("Starting database migration for {DbContext}", name);
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Database migration for {DbContext} completed successfully", name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database migration for {DbContext} failed", name);
            throw;
        }
    }
}