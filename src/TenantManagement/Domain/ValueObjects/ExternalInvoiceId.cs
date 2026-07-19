using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated external invoice identifier.
/// </summary>
public readonly record struct ExternalInvoiceId
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 255;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private ExternalInvoiceId(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="ExternalInvoiceId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw external invoice identifier.</param>
    /// <returns>The validated value object.</returns>
    public static ExternalInvoiceId Of(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new InvoiceException("External invoice ID cannot be empty.");
        }

        var normalizedId = id.Trim();

        if (normalizedId.Length > MaxLength)
        {
            throw new InvoiceException($"External invoice ID cannot exceed {MaxLength} characters.");
        }

        return new(normalizedId);
    }
}