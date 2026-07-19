using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.Entities;
using HauteCouture.TenantManagement.Domain.Exceptions;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Aggregates;

/// <summary>
///     Represents a purchasable subscription plan.
/// </summary>
public sealed class SubscriptionPlan : AuditableEntity<PlanId>
{
    /// <summary>
    ///     The unique code identifying the plan.
    /// </summary>
    public PlanCode Code { get; private set; }

    /// <summary>
    ///     The plan's display name.
    /// </summary>
    public PlanName Name { get; private set; }

    /// <summary>
    ///     The price charged for a monthly billing interval.
    /// </summary>
    public Money MonthlyPrice { get; private set; }

    /// <summary>
    ///     The price charged for a yearly billing interval.
    /// </summary>
    public Money YearlyPrice { get; private set; }

    /// <summary>
    ///     The usage limits enforced for tenants on this plan.
    /// </summary>
    public PlanLimits Limits { get; private set; } = null!;

    private readonly List<PlanFeature> _features = [];

    /// <summary>
    ///     The features included with this plan.
    /// </summary>
    public IReadOnlyCollection<PlanFeature> Features => _features.AsReadOnly();

    /// <summary>
    ///     Whether the plan is currently active and available for new subscriptions.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private SubscriptionPlan()
    {
    }

    private SubscriptionPlan(
        PlanId id,
        PlanCode code,
        PlanName name,
        Money monthlyPrice,
        Money yearlyPrice,
        PlanLimits limits,
        bool isActive)
    {
        Id = id;
        Code = code;
        Name = name;
        MonthlyPrice = monthlyPrice;
        YearlyPrice = yearlyPrice;
        Limits = limits;
        IsActive = isActive;
    }

    /// <summary>
    ///     Creates a new active <see cref="SubscriptionPlan"/> with the specified pricing and limits.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="code">The unique code identifying the plan.</param>
    /// <param name="name">The plan's display name.</param>
    /// <param name="monthlyPrice">The monthly price amount.</param>
    /// <param name="yearlyPrice">The yearly price amount.</param>
    /// <param name="currency">The currency shared by both prices.</param>
    /// <param name="maxUsers">The maximum number of users allowed.</param>
    /// <param name="maxDomains">The maximum number of custom domains allowed.</param>
    /// <param name="maxStorageInGb">The maximum storage, in gigabytes, allowed.</param>
    /// <param name="maxApiRequestsPerMonth">The maximum number of API requests allowed per month.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="SubscriptionPlan"/>, active by default.</returns>
    public static SubscriptionPlan Create(
        Guid id,
        string code,
        string name,
        decimal monthlyPrice,
        decimal yearlyPrice,
        string currency,
        int maxUsers,
        int maxDomains,
        int maxStorageInGb,
        int maxApiRequestsPerMonth,
        DateTimeOffset createdAt)
    {
        if (yearlyPrice < monthlyPrice)
        {
            throw new SubscriptionPlanException(
                "Yearly price cannot be lower than the monthly price.");
        }

        var subscriptionPlan = new SubscriptionPlan(
            PlanId.Of(id),
            PlanCode.Of(code),
            PlanName.Of(name),
            Money.Of(monthlyPrice, currency),
            Money.Of(yearlyPrice, currency),
            PlanLimits.Of(
                maxUsers,
                maxDomains,
                maxStorageInGb,
                maxApiRequestsPerMonth),
            true);

        subscriptionPlan.MarkAsCreated(createdAt);

        return subscriptionPlan;
    }

    /// <summary>
    ///     Adds a feature to the plan.
    /// </summary>
    /// <param name="id">The unique identifier for the feature.</param>
    /// <param name="key">The unique key identifying the feature within the plan.</param>
    /// <param name="description">The descriptive text for the feature.</param>
    /// <param name="addedAt">The timestamp to record as the addition time.</param>
    public void AddFeature(
        Guid id,
        string key,
        string description,
        DateTimeOffset addedAt)
    {
        if (!IsActive)
        {
            throw new SubscriptionPlanException(
                "Cannot add features to a deprecated subscription plan.");
        }
        if (addedAt < CreatedAt)
        {
            throw new SubscriptionPlanException(
                "Feature addition date cannot be earlier than the subscription plan creation date.");
        }

        var featureKey = FeatureKey.Of(key);

        if (_features.Any(f => f.Key == featureKey))
        {
            throw new SubscriptionPlanException($"Feature '{featureKey.Value}' has already been added.");
        }

        var feature = PlanFeature.Create(
            id,
            featureKey.Value,
            description,
            addedAt);

        _features.Add(feature);
        MarkAsUpdated(addedAt);
    }

    /// <summary>
    ///     Activates the plan, making it available for new subscriptions.
    /// </summary>
    /// <param name="activatedAt">The timestamp to record as the activation time.</param>
    public void Activate(DateTimeOffset activatedAt)
    {
        if (IsActive)
        {
            throw new SubscriptionPlanException("Subscription plan is already active.");
        }
        if (activatedAt < CreatedAt)
        {
            throw new SubscriptionPlanException(
                "Plan activation date cannot be earlier than the subscription plan creation date.");
        }

        IsActive = true;
        MarkAsUpdated(activatedAt);
    }

    /// <summary>
    ///     Deprecates the plan, preventing new subscriptions and feature additions.
    /// </summary>
    /// <param name="deprecatedAt">The timestamp to record as the deprecation time.</param>
    public void Deprecate(DateTimeOffset deprecatedAt)
    {
        if (!IsActive)
        {
            throw new SubscriptionPlanException("Subscription plan has already been deprecated.");
        }
        if (deprecatedAt < CreatedAt)
        {
            throw new SubscriptionPlanException(
                "Plan deprecation date cannot be earlier than the subscription plan creation date.");
        }

        IsActive = false;
        MarkAsUpdated(deprecatedAt);
    }
}