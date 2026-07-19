using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Entities;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="TenantDomain"/>.
/// </summary>
public sealed class TenantDomainConfiguration : IEntityTypeConfiguration<TenantDomain>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<TenantDomain> builder)
    {
        builder.ToTable("TenantDomains");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasConversion(
                id => id.Value,
                value => DomainId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(d => d.Name)
            .HasConversion(
                name => name.Value,
                value => DomainName.Of(value))
            .HasMaxLength(DomainName.MaxLength)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(d => d.VerificationToken)
            .HasConversion(
                token => token.Value,
                value => DomainVerificationToken.Of(value))
            .HasMaxLength(DomainVerificationToken.MaxLength)
            .IsRequired();

        builder.Property(d => d.VerifiedAt)
            .HasConversion(
                date => date == null ? (DateTimeOffset?)null : date.Value.Value,
                value => value == null ? null : DomainVerificationDate.Of(value.Value))
            .IsRequired(false);

        builder.Property<Guid>("TenantId")
            .IsRequired();

        builder.HasIndex("TenantId", nameof(TenantDomain.Name))
            .IsUnique();

        builder.ConfigureAuditableEntity<TenantDomain, DomainId>();
    }
}