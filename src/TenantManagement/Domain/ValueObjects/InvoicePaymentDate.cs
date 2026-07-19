using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated payment date of an invoice.
/// </summary>
public readonly record struct InvoicePaymentDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private InvoicePaymentDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="InvoicePaymentDate"/> from the specified raw value.
    /// </summary>
    /// <param name="paidAt">The raw payment date.</param>
    /// <returns>The validated value object.</returns>
    public static InvoicePaymentDate Of(DateTimeOffset paidAt)
    {
        if (paidAt == default)
        {
            throw new InvoiceException("Invoice payment date must be specified.");
        }
        if (paidAt > DateTimeOffset.UtcNow)
        {
            throw new InvoiceException("Invoice payment date cannot be in the future.");
        }

        return new(paidAt);
    }
}