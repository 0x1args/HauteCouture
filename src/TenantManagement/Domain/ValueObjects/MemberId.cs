using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of a tenant member.
/// </summary>
public readonly record struct MemberId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private MemberId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="MemberId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw member identifier.</param>
    /// <returns>The validated value object.</returns>
    public static MemberId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new TenantMemberException("Member ID cannot be empty.");
        }

        return new(id);
    }
}