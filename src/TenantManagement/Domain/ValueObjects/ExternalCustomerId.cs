using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated external customer identifier.
/// </summary>
public readonly record struct ExternalCustomerId
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 255;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private ExternalCustomerId(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="ExternalCustomerId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw external customer identifier.</param>
    /// <returns>The validated value object.</returns>
    public static ExternalCustomerId Of(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new SubscriptionException("External customer ID cannot be empty.");
        }

        var normalizedId = id.Trim();

        if (normalizedId.Length > MaxLength)
        {
            throw new SubscriptionException($"External customer ID cannot exceed {MaxLength} characters.");
        }

        return new(normalizedId);
    }
}