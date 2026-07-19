using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique identifier of a subscription plan.
/// </summary>
public readonly record struct PlanId
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public Guid Value { get; }

    private PlanId(Guid id) => Value = id;

    /// <summary>
    ///     Creates a <see cref="PlanId"/> from the specified raw value.
    /// </summary>
    /// <param name="id">The raw plan identifier.</param>
    /// <returns>The validated value object.</returns>
    public static PlanId Of(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new SubscriptionPlanException("Plan ID cannot be empty.");
        }

        return new(id);
    }
}