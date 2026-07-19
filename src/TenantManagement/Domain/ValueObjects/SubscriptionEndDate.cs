using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated end date of a subscription.
/// </summary>
public readonly record struct SubscriptionEndDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private SubscriptionEndDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="SubscriptionEndDate"/> from the specified raw value.
    /// </summary>
    /// <param name="value">The raw end date.</param>
    /// <returns>The validated value object.</returns>
    public static SubscriptionEndDate Of(DateTimeOffset value)
    {
        if (value == default)
        {
            throw new SubscriptionException("Subscription end date must be specified.");
        }

        return new(value);
    }
}