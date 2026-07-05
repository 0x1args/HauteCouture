using HauteCouture.Shared.Databases.Postgres.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HauteCouture.Shared.Databases.Postgres.Configurators;

/// <summary>
///     Base <see cref="DbContext"/> configurator.
/// </summary>
/// <typeparam name="TDbContext">Type of <see cref="DbContext"/>. </typeparam>
public abstract class BaseDbContextConfigurator<TDbContext>(
    IConfiguration configuration,
    ILoggerFactory loggerFactory)
    : IDbContextConfigurator<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     Database connection string name. 
    /// </summary>
    protected abstract string ConnectionStringName { get; }

    /// <summary>
    ///     Maximum time that a database command is allowed to execute before timing out.
    /// </summary>
    protected virtual int CommandTimeoutSeconds => 60;

    /// <summary>
    ///     Maximum number of SQL statements sent to the database in a single round-trip during. 
    /// </summary>
    protected int MaxBatchSize => 256;

    /// <summary>
    ///     Maximum number of retry attempts for transient database failures.
    /// </summary>
    protected virtual int MaxRetryCount => 3;

    /// <summary>
    ///     Maximum delay between retry attempts for transient database failures. 
    /// </summary>
    protected virtual TimeSpan MaxRetryDelay => TimeSpan.FromSeconds(10);

    /// <summary>
    ///     Minimum execution time required for a query to be considered slow. 
    /// </summary>
    protected virtual TimeSpan SlowQueryThreshold => TimeSpan.FromSeconds(2);

    /// <summary>
    ///     Indicates whether sensitive data logging should be enabled. 
    /// </summary>
    protected virtual bool EnableSensitiveDataLogging => false;

    /// <summary>
    ///     Indicates whether detailed error messages should be enabled. 
    /// </summary>
    protected virtual bool EnableDetailedErrors => false;

    /// <summary>
    ///     Returns additional interceptors.
    /// </summary>
    protected virtual IEnumerable<IInterceptor> GetInterceptors() => [];

    /// <inheritdoc/>
    public void Configure(DbContextOptionsBuilder<TDbContext> optionsBuilder)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException($"Connection string '{ConnectionStringName}' could not be found.");
        }

        var interceptors = new List<IInterceptor>(GetInterceptors())
        {
            new SlowQueryInterceptor(
                loggerFactory.CreateLogger<SlowQueryInterceptor>(),
                SlowQueryThreshold)
        };

        optionsBuilder
            .UseLoggerFactory(loggerFactory)
            .UseSnakeCaseNamingConvention()
            .UseNpgsql(connectionString, builder =>
            {
                builder
                    .CommandTimeout(CommandTimeoutSeconds)
                    .EnableRetryOnFailure(
                        maxRetryCount: MaxRetryCount,
                        maxRetryDelay: MaxRetryDelay,
                        errorCodesToAdd: null)
                    .MaxBatchSize(MaxBatchSize);
            })
            .AddInterceptors(interceptors);

        if (EnableSensitiveDataLogging)
        {
            optionsBuilder.EnableSensitiveDataLogging(EnableSensitiveDataLogging);
        }
        if (EnableDetailedErrors)
        {
            optionsBuilder.EnableDetailedErrors(EnableDetailedErrors);
        }
    }
}