using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of a tenant.
/// </summary>
public readonly record struct TenantId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private TenantId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="TenantId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw tenant identifier.</param>
    /// <returns>The validated value object.</returns>
    public static TenantId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new TenantException("Tenant ID cannot be empty.");
        }

        return new(id);
    }
}