using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace HauteCouture.Shared.Databases.Postgres.Interceptors;

/// <summary>
///     Interceptor for detecting slow queries.
/// </summary>
public sealed class SlowQueryInterceptor(
     ILogger<SlowQueryInterceptor> logger,
     TimeSpan threshold) : DbCommandInterceptor
{
    /// <inheritdoc/>
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogIfSlow(command, eventData.Duration);
        return result;
    }

    /// <inheritdoc/>
    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData.Duration);
        return ValueTask.FromResult(result);
    }

    private void LogIfSlow(DbCommand command, TimeSpan duration)
    {
        if (duration <= threshold)
        {
            return;
        }

        logger.LogWarning(
            "Slow query detected ({Duration}ms > {Threshold}ms):\n{CommandText}",
            duration.TotalMilliseconds,
            threshold.TotalMilliseconds,
            command.CommandText);
    }
}