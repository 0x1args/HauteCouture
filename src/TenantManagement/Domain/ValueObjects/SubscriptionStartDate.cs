using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated start date of a subscription.
/// </summary>
public readonly record struct SubscriptionStartDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private SubscriptionStartDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="SubscriptionStartDate"/> from the specified raw value.
    /// </summary>
    /// <param name="value">The raw start date.</param>
    /// <returns>The validated value object.</returns>
    public static SubscriptionStartDate Of(DateTimeOffset value)
    {
        if (value == default)
        {
            throw new SubscriptionException("Subscription start date must be specified.");
        }

        return new(value);
    }
}