using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Entities;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="PlanFeature"/>.
/// </summary>
public sealed class PlanFeatureConfiguration : IEntityTypeConfiguration<PlanFeature>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<PlanFeature> builder)
    {
        builder.ToTable("PlanFeatures");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id)
            .HasConversion(
                id => id.Value,
                value => FeatureId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(f => f.Key)
            .HasConversion(
                key => key.Value,
                value => FeatureKey.Of(value))
            .HasMaxLength(FeatureKey.MaxLength)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasConversion(
                desc => desc.Value,
                value => FeatureDescription.Of(value))
            .HasMaxLength(FeatureDescription.MaxLength)
            .IsRequired();

        builder.Property<Guid>("PlanId")
            .IsRequired();

        builder.HasIndex("PlanId", nameof(PlanFeature.Key))
            .IsUnique();

        builder.ConfigureAuditableEntity<PlanFeature, FeatureId>();
    }
}