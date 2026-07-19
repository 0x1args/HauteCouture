using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated verification date of a domain.
/// </summary>
public readonly record struct DomainVerificationDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private DomainVerificationDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="DomainVerificationDate"/> from the specified raw value.
    /// </summary>
    /// <param name="verifiedAt">The raw verification date.</param>
    /// <returns>The validated value object.</returns>
    public static DomainVerificationDate Of(DateTimeOffset verifiedAt)
    {
        if (verifiedAt == default)
        {
            throw new TenantDomainException("Domain verification date must be specified.");
        }
        if (verifiedAt > DateTimeOffset.UtcNow)
        {
            throw new TenantDomainException("Domain verification date cannot be in the future.");
        }

        return new(verifiedAt);
    }
}