using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated display name of a tenant.
/// </summary>
public readonly record struct TenantName
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private TenantName(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="TenantName"/> from the specified raw value.
    /// </summary>
    /// <param name="name">The raw tenant name.</param>
    /// <returns>The validated value object.</returns>
    public static TenantName Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new TenantException("Tenant name cannot be empty.");
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxLength)
        {
            throw new TenantException($"Tenant name cannot exceed {MaxLength} characters.");
        }

        return new(normalizedName);
    }
}