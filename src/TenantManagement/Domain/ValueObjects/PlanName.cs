using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated display name of a subscription plan.
/// </summary>
public readonly record struct PlanName
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private PlanName(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="PlanName"/> from the specified raw value.
    /// </summary>
    /// <param name="name">The raw plan name.</param>
    /// <returns>The validated value object.</returns>
    public static PlanName Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SubscriptionPlanException("Plan name cannot be empty.");
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxLength)
        {
            throw new SubscriptionPlanException($"Plan name cannot exceed {MaxLength} characters.");
        }

        return new(normalizedName);
    }
}