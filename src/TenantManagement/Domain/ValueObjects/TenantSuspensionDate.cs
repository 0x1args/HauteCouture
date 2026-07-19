using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated suspension date of a tenant.
/// </summary>
public readonly record struct TenantSuspensionDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private TenantSuspensionDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="TenantSuspensionDate"/> from the specified raw value.
    /// </summary>
    /// <param name="suspendedAt">The raw suspension date.</param>
    /// <returns>The validated value object.</returns>
    public static TenantSuspensionDate Of(DateTimeOffset suspendedAt)
    {
        if (suspendedAt == default)
        {
            throw new TenantException("Tenant suspension date must be specified.");
        }
        if (suspendedAt > DateTimeOffset.UtcNow)
        {
            throw new TenantException("Tenant suspension date cannot be in the future.");
        }

        return new(suspendedAt);
    }
}