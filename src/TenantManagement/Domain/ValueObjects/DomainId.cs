using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of a domain.
/// </summary>
public readonly record struct DomainId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private DomainId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="DomainId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw domain identifier.</param>
    /// <returns>The validated value object.</returns>
    public static DomainId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new TenantDomainException("Domain ID cannot be empty.");
        }

        return new(id);
    }
}