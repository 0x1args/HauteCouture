using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated removal date of a member.
/// </summary>
public readonly record struct MemberRemovalDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private MemberRemovalDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="MemberRemovalDate"/> from the specified raw value.
    /// </summary>
    /// <param name="verifiedAt">The raw removal date.</param>
    /// <returns>The validated value object.</returns>
    public static MemberRemovalDate Of(DateTimeOffset verifiedAt)
    {
        if (verifiedAt == default)
        {
            throw new TenantMemberException("Member removal date must be specified.");
        }
        if (verifiedAt > DateTimeOffset.UtcNow)
        {
            throw new TenantMemberException("Member removal date cannot be in the future.");
        }

        return new(verifiedAt);
    }
}