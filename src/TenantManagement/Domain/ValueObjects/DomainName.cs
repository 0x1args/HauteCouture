using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated custom domain name.
/// </summary>
public readonly record struct DomainName
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 253;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private DomainName(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="DomainName"/> from the specified raw value.
    /// </summary>
    /// <param name="name">The raw domain name.</param>
    /// <returns>The validated value object.</returns>
    public static DomainName Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new TenantDomainException("Custom domain name cannot be empty.");
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxLength)
        {
            throw new TenantDomainException($"Custom domain name cannot exceed {MaxLength} characters.");
        }
        if (!Uri.CheckHostName(normalizedName).Equals(UriHostNameType.Dns))
        {
            throw new TenantDomainException("Custom domain name is not a valid DNS name.");
        }

        return new(normalizedName);
    }
}