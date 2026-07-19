using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated trial end date of a subscription.
/// </summary>
public readonly record struct SubscriptionTrialEndDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private SubscriptionTrialEndDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="SubscriptionTrialEndDate"/> from the specified raw value.
    /// </summary>
    /// <param name="value">The raw trial end date.</param>
    /// <returns>The validated value object.</returns>
    public static SubscriptionTrialEndDate Of(DateTimeOffset value)
    {
        if (value == default)
        {
            throw new SubscriptionException("Trial end date must be specified.");
        }

        return new(value);
    }
}