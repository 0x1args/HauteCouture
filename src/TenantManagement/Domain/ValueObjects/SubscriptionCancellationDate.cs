using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated cancellation date of a subscription.
/// </summary>
public readonly record struct SubscriptionCancellationDate
{
    /// <summary>
    ///     The underlying value.
    /// </summary>
    public DateTimeOffset Value { get; }

    private SubscriptionCancellationDate(DateTimeOffset value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="SubscriptionCancellationDate"/> from the specified raw value.
    /// </summary>
    /// <param name="value">The raw cancellation date.</param>
    /// <returns>The validated value object.</returns>
    public static SubscriptionCancellationDate Of(DateTimeOffset value)
    {
        if (value == default)
        {
            throw new SubscriptionException("Subscription cancellation date must be specified.");
        }
        if (value > DateTimeOffset.UtcNow)
        {
            throw new SubscriptionException("Subscription cancellation date cannot be in the future.");
        }

        return new(value);
    }
}