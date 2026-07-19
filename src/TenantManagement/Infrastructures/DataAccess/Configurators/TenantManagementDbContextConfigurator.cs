using HauteCouture.Shared.Databases.Postgres.Configurators;
using HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.Configurators;

/// <summary>
///     Configurator for <see cref="TenantManagementDbContext"/> configurator.
/// </summary>
public sealed class TenantManagementDbContextConfigurator(
    IConfiguration configuration,
    ILoggerFactory loggerFactory)
    : BaseDbContextConfigurator<TenantManagementDbContext>(configuration, loggerFactory)
{
    /// <inheritdoc />
    protected override string ConnectionStringName => "TenantManagement";

    /// <inheritdoc />
    protected override bool EnableDetailedErrors => true;

    /// <inheritdoc />
    protected override bool EnableSensitiveDataLogging => true;
}