using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of an invitation.
/// </summary>
public readonly record struct InvitationId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private InvitationId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="InvitationId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw invitation identifier.</param>
    /// <returns>The validated value object.</returns>
    public static InvitationId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new TenantInvitationException("Invitation ID cannot be empty.");
        }

        return new(id);
    }
}