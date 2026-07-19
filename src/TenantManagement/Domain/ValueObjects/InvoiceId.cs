using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of an invoice.
/// </summary>
public readonly record struct InvoiceId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private InvoiceId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="InvoiceId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw invoice identifier.</param>
    /// <returns>The validated value object.</returns>
    public static InvoiceId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new TenantDomainException("Invoice ID cannot be empty.");
        }

        return new(id);
    }
}