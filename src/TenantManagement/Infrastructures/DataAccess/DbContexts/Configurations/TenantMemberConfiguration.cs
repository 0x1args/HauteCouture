using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Aggregates;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="TenantMember"/>.
/// </summary>
public sealed class TenantMemberConfiguration : IEntityTypeConfiguration<TenantMember>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TenantMember> builder)
    {
        builder.ToTable("TenantMembers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasConversion(
                id => id.Value,
                value => MemberId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(m => m.TenantId)
            .HasConversion(
                id => id.Value,
                value => TenantId.Of(value))
            .IsRequired();

        builder.Property(m => m.UserId)
            .HasConversion(
                id => id.Value,
                value => MemberUserId.Of(value))
            .IsRequired();

        builder.Property(m => m.Role)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(m => m.RemovedAt)
            .HasConversion(
                date => date == null ? (DateTimeOffset?)null : date.Value.Value,
                value => value == null ? null : MemberRemovalDate.Of(value.Value))
            .IsRequired(false);

        builder.HasIndex(m => new { m.TenantId, m.UserId })
           .IsUnique();

        builder.ConfigureAuditableEntity<TenantMember, MemberId>();
    }
}