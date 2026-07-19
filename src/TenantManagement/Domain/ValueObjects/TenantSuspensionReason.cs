using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated suspension reason for a tenant.
/// </summary>
public readonly record struct TenantSuspensionReason
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 255;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private TenantSuspensionReason(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="TenantSuspensionReason"/> from the specified raw value.
    /// </summary>
    /// <param name="reason">The raw suspension reason.</param>
    /// <returns>The validated value object.</returns>
    public static TenantSuspensionReason Of(string reason)
    {
        if (string.IsNullOrEmpty(reason))
        {
            throw new TenantException("Tenant suspension reason cannot be empty.");
        }

        var normalizedReason = reason.Trim();

        if (normalizedReason.Length > MaxLength)
        {
            throw new TenantException($"Tenant suspension reason cannot exceed {MaxLength} characters.");
        }

        return new(normalizedReason);
    }
}