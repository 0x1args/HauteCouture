using HauteCouture.Example.Infrastuctures.DataAccess.DbContexts;
using HauteCouture.Shared.Databases.Postgres.Configurators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HauteCouture.Example.Infrastuctures.DataAccess.Configurators;

/// <summary>
///     Configurator for <see cref="SomethingDbContext"/> configurator.
/// </summary>
public sealed class SomethingDbContextConfigurator(
    IConfiguration configuration,
    ILoggerFactory loggerFactory)
    : BaseDbContextConfigurator<SomethingDbContext>(configuration, loggerFactory)
{
    /// <inheritdoc />
    protected override string ConnectionStringName => "Postgres";

    /// <inheritdoc />
    protected override bool EnableDetailedErrors => true;

    /// <inheritdoc />
    protected override bool EnableSensitiveDataLogging => true;
}