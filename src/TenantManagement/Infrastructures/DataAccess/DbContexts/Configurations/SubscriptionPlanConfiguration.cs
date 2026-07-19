using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Aggregates;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="SubscriptionPlan"/>.
/// </summary>
public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("SubscriptionPlans");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => PlanId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(p => p.Code)
            .HasConversion(
                code => code.Value,
                value => PlanCode.Of(value))
            .HasMaxLength(PlanCode.MaxLength)
            .IsRequired();

        builder.Property(p => p.Name)
            .HasConversion(
                name => name.Value,
                value => PlanName.Of(value))
            .HasMaxLength(PlanName.MaxLength)
            .IsRequired();

        builder.Property(p => p.MonthlyPrice)
            .HasConversion(
                money => JsonSerializer.Serialize(money, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<Money>(json, (JsonSerializerOptions?)null)!)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(p => p.YearlyPrice)
            .HasConversion(
                money => JsonSerializer.Serialize(money, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<Money>(json, (JsonSerializerOptions?)null)!)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(p => p.Limits)
            .HasConversion(
                limits => JsonSerializer.Serialize(limits, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<PlanLimits>(json, (JsonSerializerOptions?)null)!)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.HasMany(p => p.Features)
            .WithOne()
            .HasForeignKey("PlanId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.Code)
          .IsUnique();

        builder.ConfigureAuditableEntity<SubscriptionPlan, PlanId>();
    }
}