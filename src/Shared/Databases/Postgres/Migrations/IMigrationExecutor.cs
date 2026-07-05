using Microsoft.Extensions.Logging;

namespace HauteCouture.Shared.Databases.Postgres.Migrations;

/// <summary>
///     Executor for database migrations.
/// </summary>
public interface IMigrationExecutor
{
    /// <summary>
    ///     Applies all pending migrations to the database.
    /// </summary>
    /// <param name="logger">Logger used to report migration progress and errors. </param>
    /// <param name="cancellationToken"> Token to cancel the operation. </param>
    Task MigrateAsync(ILogger logger, CancellationToken cancellationToken);
}