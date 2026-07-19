using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Aggregates;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="Subscription"/>.
/// </summary>
public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasConversion(
                id => id.Value,
                value => SubscriptionId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(s => s.TenantId)
            .HasConversion(
                id => id.Value,
                value => TenantId.Of(value))
            .IsRequired();

        builder.Property(s => s.PlanId)
            .HasConversion(
                id => id.Value,
                value => PlanId.Of(value))
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.Interval)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(s => s.CurrentPeriodStart)
            .HasConversion(
                date => date.Value,
                value => SubscriptionStartDate.Of(value))
            .IsRequired();

        builder.Property(s => s.CurrentPeriodEnd)
            .HasConversion(
                date => date.Value,
                value => SubscriptionEndDate.Of(value))
            .IsRequired();

        builder.Property(s => s.TrialEndsAt)
            .HasConversion(
                date => date == null ? (DateTimeOffset?)null : date.Value.Value,
                value => value == null ? null : SubscriptionTrialEndDate.Of(value.Value))
            .IsRequired(false);

        builder.Property(s => s.CancelledAt)
            .HasConversion(
                date => date == null ? (DateTimeOffset?)null : date.Value.Value,
                value => value == null ? null : SubscriptionCancellationDate.Of(value.Value))
            .IsRequired(false);

        builder.Property(s => s.CancelAtPeriodEnd)
            .IsRequired();

        var customerConverter = new ValueConverter<ExternalCustomerId?, string?>(
            id => id.HasValue ? id.Value.Value : null,
            value => value == null ? null : ExternalCustomerId.Of(value));

        builder.Property(s => s.ExternalProviderCustomerId)
            .HasConversion(customerConverter)
            .HasMaxLength(ExternalCustomerId.MaxLength)
            .IsRequired(false);

        var subscriptionConverter = new ValueConverter<ExternalSubscriptionId?, string?>(
            id => id.HasValue ? id.Value.Value : null,
            value => value == null ? null : ExternalSubscriptionId.Of(value));

        builder.Property(s => s.ExternalProviderSubscriptionId)
            .HasConversion(subscriptionConverter)
            .HasMaxLength(ExternalSubscriptionId.MaxLength)
            .IsRequired(false);

        builder.HasIndex(s => s.ExternalProviderSubscriptionId)
            .IsUnique()
            .HasFilter("\"ExternalProviderSubscriptionId\" IS NOT NULL");

        builder.HasIndex(s => s.TenantId);

        builder.ConfigureAuditableEntity<Subscription, SubscriptionId>();
    }
}