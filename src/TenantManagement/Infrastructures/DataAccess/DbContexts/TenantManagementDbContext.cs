using Microsoft.EntityFrameworkCore;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts;

/// <summary>
///     EF Core database context for the TenantManagement.
/// </summary>
public sealed class TenantManagementDbContext(
    DbContextOptions<TenantManagementDbContext> options)
    : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

    }
}