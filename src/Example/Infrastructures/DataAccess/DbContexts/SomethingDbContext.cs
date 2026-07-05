using Microsoft.EntityFrameworkCore;

namespace HauteCouture.Example.Infrastuctures.DataAccess.DbContexts;

/// <summary>
///     EF Core database context for the Example.
/// </summary>
public sealed class SomethingDbContext(
    DbContextOptions<SomethingDbContext> options)
    : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        CustomModelBuilder.Configure(modelBuilder);
    }
}