using HauteCouture.Example.Infrastuctures.DataAccess.DbContexts;
using HauteCouture.Shared.Databases.Postgres.Configurators;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HauteCouture.Example.Infrastuctures.DataAccess.Configurators;

public sealed class SomethingDbContextConfigurator(
    IConfiguration configuration,
    ILoggerFactory loggerFactory)
    : BaseDbContextConfigurator<SomethingDbContext>(configuration, loggerFactory)
{
    protected override string ConnectionStringName => "Postgres";

    protected override bool EnableDetailedErrors => true;

    protected override bool EnableSensitiveDataLogging => true;
}