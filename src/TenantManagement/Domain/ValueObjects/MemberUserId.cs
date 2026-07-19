using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated user identifier associated with a member.
/// </summary>
public readonly record struct MemberUserId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private MemberUserId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="MemberUserId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw user identifier.</param>
    /// <returns>The validated value object.</returns>
    public static MemberUserId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new TenantMemberException("Member user ID cannot be empty.");
        }

        return new(id);
    }
}