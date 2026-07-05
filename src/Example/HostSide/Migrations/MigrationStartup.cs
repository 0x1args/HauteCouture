using HauteCouture.Shared.Databases.Postgres.Migrations;
using Microsoft.Extensions.Logging;

namespace HauteCouture.Example.HostSide.Private.Migrations;

/// <summary>
///     Applies pending EF Core migrations for <see cref="SomethingDbContext"/> at host startup.
/// </summary>
public sealed class MigrationStartup(
    IMigrationExecutor migrationExecutor,
    ILoggerFactory loggerFactory)
{
    /// <summary>
    ///     Executes pending migrations against the database.
    /// </summary>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger<MigrationStartup>();
        await migrationExecutor.MigrateAsync(logger, cancellationToken);
    }
}