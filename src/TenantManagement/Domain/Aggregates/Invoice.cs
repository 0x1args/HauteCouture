using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.Enums;
using HauteCouture.TenantManagement.Domain.Exceptions;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Aggregates;

/// <summary>
///     Represents a billing invoice issued for a tenant's subscription by the external
///     payment provider.
/// </summary>
public sealed class Invoice : AuditableEntity<InvoiceId>
{
    /// <summary>
    ///     The identifier of the billed tenant.
    /// </summary>
    public TenantId TenantId { get; private set; }

    /// <summary>
    ///     The identifier of the subscription the invoice was issued for.
    /// </summary>
    public SubscriptionId SubscriptionId { get; private set; }

    /// <summary>
    ///     The invoiced amount.
    /// </summary>
    public Money Amount { get; private set; }

    /// <summary>
    ///     The current status of the invoice.
    /// </summary>
    public InvoiceStatus Status { get; private set; }

    /// <summary>
    ///     The timestamp at which the invoice was paid, if paid.
    /// </summary>
    public InvoicePaymentDate? PaidAt { get; private set; }

    /// <summary>
    ///     The identifier of the corresponding invoice record in the external billing provider.
    /// </summary>
    public ExternalInvoiceId ExternalProviderInvoiceId { get; private set; }

    /// <summary>
    ///     The URL of the invoice's PDF document, if paid.
    /// </summary>
    public InvoicePdfUrl? PdfUrl { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private Invoice()
    {
    }

    private Invoice(
        InvoiceId id,
        TenantId tenantId,
        SubscriptionId subscriptionId,
        Money amount,
        InvoiceStatus status,
        ExternalInvoiceId externalProviderInvoiceId)
    {
        Id = id;
        TenantId = tenantId;
        SubscriptionId = subscriptionId;
        Amount = amount;
        Status = status;
        ExternalProviderInvoiceId = externalProviderInvoiceId;
    }

    /// <summary>
    ///     Creates a new <see cref="Invoice"/> from data received from the external billing provider.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="tenantId">The identifier of the billed tenant.</param>
    /// <param name="subscriptionId">The identifier of the related subscription.</param>
    /// <param name="amount">The invoiced amount.</param>
    /// <param name="currency">The currency of the invoiced amount.</param>
    /// <param name="providerInvoiceId">The identifier assigned by the external provider.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="Invoice"/>, in the <see cref="InvoiceStatus.Open"/> status.</returns>
    public static Invoice CreateFromProvider(
        Guid id,
        Guid tenantId,
        Guid subscriptionId,
        decimal amount,
        string currency,
        string providerInvoiceId,
        DateTimeOffset createdAt)
    {
        var invoice = new Invoice(
            InvoiceId.Of(id),
            TenantId.Of(tenantId),
            SubscriptionId.Of(subscriptionId),
            Money.Of(amount, currency),
            InvoiceStatus.Open,
            ExternalInvoiceId.Of(providerInvoiceId));

        invoice.MarkAsCreated(createdAt);

        return invoice;
    }

    /// <summary>
    ///     Marks the invoice as paid.
    /// </summary>
    /// <param name="pdfUrl">The URL of the invoice's PDF document.</param>
    /// <param name="paidAt">The timestamp to record as the payment time.</param>
    public void MarkPaid(
        string pdfUrl,
        DateTimeOffset paidAt)
    {
        EnsureStatusTransitionAllowedTo(InvoiceStatus.Paid);

        if (paidAt < CreatedAt)
        {
            throw new InvoiceException(
                "Invoice payment date cannot be earlier than the invoice creation date.");
        }

        Status = InvoiceStatus.Paid;
        PaidAt = InvoicePaymentDate.Of(paidAt);
        PdfUrl = InvoicePdfUrl.Of(pdfUrl);
        MarkAsUpdated(paidAt);
    }

    /// <summary>
    ///     Marks the invoice as failed, typically after a declined payment attempt.
    /// </summary>
    /// <param name="failedAt">The timestamp to record as the failure time.</param>
    public void MarkFailed(
        DateTimeOffset failedAt)
    {
        EnsureStatusTransitionAllowedTo(InvoiceStatus.Failed);

        if (failedAt < CreatedAt)
        {
            throw new InvoiceException(
                "Invoice failure date cannot be earlier than the invoice creation date.");
        }

        Status = InvoiceStatus.Failed;
        MarkAsUpdated(failedAt);
    }

    private void EnsureStatusTransitionAllowedTo(
        InvoiceStatus targetStatus)
    {
        var allowed = (Status, targetStatus) switch
        {
            (InvoiceStatus.Open, InvoiceStatus.Paid) => true,
            (InvoiceStatus.Open, InvoiceStatus.Failed) => true,
            (InvoiceStatus.Failed, InvoiceStatus.Paid) => true,

            _ => false
        };

        if (!allowed)
        {
            throw new InvoiceException(
                $"Invoice status transition from '{Status}' to '{targetStatus}' is not allowed.");
        }
    }
}