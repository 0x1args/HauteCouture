using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of a feature.
/// </summary>
public readonly record struct FeatureId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private FeatureId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="FeatureId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw feature identifier.</param>
    /// <returns>The validated value object.</returns>
    public static FeatureId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new PlanFeatureException("Feature ID cannot be empty.");
        }

        return new(id);
    }
}