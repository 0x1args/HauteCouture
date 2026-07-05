using HauteCouture.Example.Infrastuctures.DataAccess.DbContexts.Configurations;
using HauteCouture.Shared.Databases.Postgres.Extensions;
using Microsoft.EntityFrameworkCore;

namespace HauteCouture.Example.Infrastuctures.DataAccess.DbContexts;

/// <summary>
///     Centralizes model-building for <see cref="SomethingDbContext"/>,
///     applying entity type configurations alongside shared infrastructure conventions.
/// </summary>
public static class CustomModelBuilder
{
    /// <summary>
    ///     Applies all entity type configurations and shared model conventions.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    public static void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new SomethingConfiguration())
            .SetDefaultDateTimeKind(DateTimeKind.Utc)
            .ApplySoftDeleteQueryFilter();
    }
}