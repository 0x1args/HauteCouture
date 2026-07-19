using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated verification token for a domain.
/// </summary>
public readonly record struct DomainVerificationToken
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 32;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private DomainVerificationToken(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="DomainVerificationToken"/> from the specified raw value.
    /// </summary>
    /// <param name="token">The raw verification token.</param>
    /// <returns>The validated value object.</returns>
    public static DomainVerificationToken Of(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new TenantDomainException("Domain verification token cannot be empty.");
        }

        var normalizedToken = token.Trim();

        if (normalizedToken.Length != MaxLength)
        {
            throw new TenantDomainException($"Domain verification token cannot exceed {MaxLength} characters.");
        }

        return new(normalizedToken);
    }

    /// <summary>
    ///     Generates a new domain verification token.
    /// </summary>
    /// <returns>A newly generated token.</returns>
    public static DomainVerificationToken Generate()
    {
        return new($"tm-verify={Guid.NewGuid():N}");
    }
}