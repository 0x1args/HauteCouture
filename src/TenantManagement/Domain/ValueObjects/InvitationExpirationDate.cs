using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated expiration date of an invitation.
/// </summary>
public readonly record struct InvitationExpirationDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private InvitationExpirationDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="InvitationExpirationDate"/> from the specified raw value.
    /// </summary>
    /// <param name="verifiedAt">The raw expiration date.</param>
    /// <returns>The validated value object.</returns>
    public static InvitationExpirationDate Of(DateTimeOffset verifiedAt)
    {
        if (verifiedAt == default)
        {
            throw new TenantInvitationException("Invitation expiration date must be specified.");
        }

        return new(verifiedAt);
    }
}