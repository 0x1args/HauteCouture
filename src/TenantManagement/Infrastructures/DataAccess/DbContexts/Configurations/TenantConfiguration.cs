using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Aggregates;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="Tenant"/>.
/// </summary>
public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => TenantId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(t => t.Name)
            .HasConversion(
                name => name.Value,
                value => TenantName.Of(value))
            .HasMaxLength(TenantName.MaxLength)
            .IsRequired();

        builder.Property(t => t.Slug)
            .HasConversion(
                slug => slug.Value,
                value => TenantSlug.Of(value))
            .HasMaxLength(TenantSlug.MaxLength)
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(t => t.Settings)
            .HasConversion(
                settings => JsonSerializer.Serialize(settings, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<TenantSettings>(json, (JsonSerializerOptions?)null)!)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(t => t.SuspendedAt)
            .HasConversion(
                date => date == null ? (DateTimeOffset?)null : date.Value.Value,
                value => value == null ? null : TenantSuspensionDate.Of(value.Value))
            .IsRequired(false);

        var suspensionReasonConverter = new ValueConverter<TenantSuspensionReason?, string?>(
            reason => reason.HasValue ? reason.Value.Value : null,
            value => value == null ? null : TenantSuspensionReason.Of(value));

        builder.Property(t => t.SuspensionReason)
            .HasConversion(suspensionReasonConverter)
            .HasMaxLength(TenantSuspensionReason.MaxLength)
            .IsRequired(false);

        builder.HasMany(t => t.Domains)
            .WithOne()
            .HasForeignKey("TenantId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.ConfigureAuditableEntity<Tenant, TenantId>();
    }
}