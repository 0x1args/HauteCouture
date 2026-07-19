using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated identifier of the inviter member.
/// </summary>
public readonly record struct InviterMemberId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private InviterMemberId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="InviterMemberId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw inviter member identifier.</param>
    /// <returns>The validated value object.</returns>
    public static InviterMemberId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new TenantInvitationException("Inviter member ID cannot be empty.");
        }

        return new(id);
    }
}