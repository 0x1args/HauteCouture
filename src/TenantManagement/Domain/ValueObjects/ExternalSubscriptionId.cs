using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated external subscription identifier.
/// </summary>
public readonly record struct ExternalSubscriptionId
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 255;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private ExternalSubscriptionId(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="ExternalSubscriptionId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw external subscription identifier.</param>
    /// <returns>The validated value object.</returns>
    public static ExternalSubscriptionId Of(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new SubscriptionException("External subscription ID cannot be empty.");
        }

        var normalizedId = id.Trim();

        if (normalizedId.Length > MaxLength)
        {
            throw new SubscriptionException($"External subscription ID cannot exceed {MaxLength} characters.");
        }

        return new(normalizedId);
    }
}