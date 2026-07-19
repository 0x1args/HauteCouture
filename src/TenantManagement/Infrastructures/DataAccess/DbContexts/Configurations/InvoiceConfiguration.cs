using HauteCouture.Shared.Databases.Postgres.Extensions;
using HauteCouture.TenantManagement.Domain.Aggregates;
using HauteCouture.TenantManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace HauteCouture.TenantManagement.Infrastructures.DataAccess.DbContexts.Configurations;

/// <summary>
///     Entity type configuration for <see cref="Invoice"/>.
/// </summary>
public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasConversion(
                id => id.Value,
                value => InvoiceId.Of(value))
            .ValueGeneratedNever()
            .IsRequired();

        builder.Property(i => i.TenantId)
            .HasConversion(
                id => id.Value,
                value => TenantId.Of(value))
            .IsRequired();

        builder.Property(i => i.SubscriptionId)
            .HasConversion(
                id => id.Value,
                value => SubscriptionId.Of(value))
            .IsRequired();

        builder.Property(i => i.Amount)
            .HasConversion(
                money => JsonSerializer.Serialize(money, (JsonSerializerOptions?)null),
                json => JsonSerializer.Deserialize<Money>(json, (JsonSerializerOptions?)null)!)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(i => i.PaidAt)
            .HasConversion(
                date => date == null ? (DateTimeOffset?)null : date.Value.Value,
                value => value == null ? null : InvoicePaymentDate.Of(value.Value))
            .IsRequired(false);

        builder.Property(i => i.ExternalProviderInvoiceId)
            .HasConversion(
                id => id.Value,
                value => ExternalInvoiceId.Of(value))
            .HasMaxLength(ExternalInvoiceId.MaxLength)
            .IsRequired();

        var pdfUrlConverter = new ValueConverter<InvoicePdfUrl?, string?>(
            url => url.HasValue ? url.Value.Value : null,
            value => value == null ? null : InvoicePdfUrl.Of(value));

        builder.Property(i => i.PdfUrl)
            .HasConversion(pdfUrlConverter)
            .HasMaxLength(InvoicePdfUrl.MaxLength)
            .IsRequired(false);

        builder.HasIndex(i => i.ExternalProviderInvoiceId)
          .IsUnique();

        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.SubscriptionId);

        builder.ConfigureAuditableEntity<Invoice, InvoiceId>();
    }
}