using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated PDF URL of an invoice.
/// </summary>
public readonly record struct InvoicePdfUrl
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 2048;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private InvoicePdfUrl(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="InvoicePdfUrl"/> from the specified raw value.
    /// </summary>
    /// <param name="url">The raw PDF URL.</param>
    /// <returns>The validated value object.</returns>
    public static InvoicePdfUrl Of(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvoiceException("Invoice PDF URL cannot be empty.");
        }

        var normalizedUrl = url.Trim();

        if (normalizedUrl.Length > MaxLength)
        {
            throw new InvoiceException($"Invoice PDF URL cannot exceed {MaxLength} characters.");
        }

        if (!Uri.TryCreate(normalizedUrl, UriKind.Absolute, out var uri))
        {
            throw new InvoiceException("Invoice PDF URL has an invalid format.");
        }

        if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvoiceException("Invoice PDF URL must use HTTP or HTTPS.");
        }

        return new(normalizedUrl);
    }
}