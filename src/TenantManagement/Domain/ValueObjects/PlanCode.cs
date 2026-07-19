using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated unique code of a subscription plan.
/// </summary>
public readonly record struct PlanCode
{
    /// <summary>
    ///     The maximum allowed length.
    /// </summary>
    public const int MaxLength = 64;

    /// <summary>
    ///     The underlying value.
    /// </summary>
    public string Value { get; }

    private PlanCode(string value) => Value = value;

    /// <summary>
    ///     Creates a <see cref="PlanCode"/> from the specified raw value.
    /// </summary>
    /// <param name="name">The raw plan code.</param>
    /// <returns>The validated value object.</returns>
    public static PlanCode Of(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new SubscriptionPlanException("Plan code cannot be empty.");
        }

        var normalizedCode = name.Trim().ToLowerInvariant();

        if (normalizedCode.Length > MaxLength)
        {
            throw new SubscriptionPlanException($"Plan code cannot exceed {MaxLength} characters.");
        }

        return new(normalizedCode);
    }
}