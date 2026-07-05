using Microsoft.EntityFrameworkCore;

namespace HauteCouture.Shared.Databases.Postgres.Configurators;

/// <summary>
///     Configurator for <see cref="DbContext"/>.
/// </summary>
/// <typeparam name="TDbContext">Type of <see cref="DbContext"/>. </typeparam>
public interface IDbContextConfigurator<TDbContext>
    where TDbContext : DbContext
{
    /// <summary>
    ///     Configure the <see cref="DbContextOptions" />.
    /// </summary>
    /// <param name="optionsBuilder">Builder used to configures the options. </param>
    void Configure(DbContextOptionsBuilder<TDbContext> optionsBuilder);
}