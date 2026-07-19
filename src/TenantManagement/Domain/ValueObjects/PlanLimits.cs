using HauteCouture.TenantManagement.Domain.Exceptions;

namespace HauteCouture.TenantManagement.Domain.ValueObjects;

/// <summary>
///     Represents the validated limits of a subscription plan.
/// </summary>
public sealed record PlanLimits
{
    public const int UnlimitedValue = -1;

    public int MaxUsers { get; }

    public int MaxDomains { get; }

    public int MaxStorageInGb { get; }

    public int MaxApiRequestsPerMonth { get; }

    private PlanLimits(
        int maxUsers,
        int maxDomains,
        int maxStorageInGb,
        int maxApiRequestsPerMonth)
    {
        MaxUsers = maxUsers;
        MaxDomains = maxDomains;
        MaxStorageInGb = maxStorageInGb;
        MaxApiRequestsPerMonth = maxApiRequestsPerMonth;
    }

    /// <summary>
    ///     Creates a <see cref="PlanLimits"/> instance with the specified values.
    /// </summary>
    /// <param name="maxUsers">Maximum number of users.</param>
    /// <param name="maxDomains">Maximum number of domains.</param>
    /// <param name="maxStorageInGb">Maximum storage in GB.</param>
    /// <param name="maxApiRequestsPerMonth">Maximum API requests per month.</param>
    /// <returns>The validated value object.</returns>
    public static PlanLimits Of(
        int maxUsers,
        int maxDomains,
        int maxStorageInGb,
        int maxApiRequestsPerMonth)
    {
        ValidateLimit(maxUsers, nameof(MaxUsers));
        ValidateLimit(maxDomains, nameof(MaxDomains));
        ValidateLimit(maxStorageInGb, nameof(MaxStorageInGb));
        ValidateLimit(maxApiRequestsPerMonth, nameof(MaxApiRequestsPerMonth));

        return new(
            maxUsers,
            maxDomains,
            maxStorageInGb,
            maxApiRequestsPerMonth);
    }

    /// <summary>
    ///     Creates a <see cref="PlanLimits"/> instance representing unlimited limits.
    /// </summary>
    /// <returns>An unlimited plan limits object.</returns>
    public static PlanLimits Unlimited() =>
        new(UnlimitedValue,
            UnlimitedValue,
            UnlimitedValue,
            UnlimitedValue);

    private static void ValidateLimit(int value, string name)
    {
        if (value is 0 or < UnlimitedValue)
        {
            throw new SubscriptionPlanException(
                $"{name} must be greater than zero or equal to {UnlimitedValue} for unlimited.");
        }
    }
}