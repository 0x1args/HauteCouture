using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Entities;

/// <summary>
///     Represents a single feature included in a <c>SubscriptionPlan</c>.
/// </summary>
public sealed class PlanFeature : AuditableEntity<FeatureId>
{
    /// <summary>
    ///     The unique key identifying the feature within its owning plan.
    /// </summary>
    public FeatureKey Key { get; private set; }

    /// <summary>
    ///     The descriptive text for the feature.
    /// </summary>
    public FeatureDescription Description { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private PlanFeature()
    {
    }

    private PlanFeature(
        FeatureId id,
        FeatureKey key,
        FeatureDescription description)
    {
        Id = id;
        Key = key;
        Description = description;
    }

    /// <summary>
    ///     Creates a new <see cref="PlanFeature"/> with the specified key and description.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="key">The unique key identifying the feature within its owning plan.</param>
    /// <param name="description">The descriptive text for the feature.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="PlanFeature"/>.</returns>
    public static PlanFeature Create(
        Guid id,
        string key,
        string description,
        DateTimeOffset createdAt)
    {
        var planFeature = new PlanFeature(
            FeatureId.Of(id),
            FeatureKey.Of(key),
            FeatureDescription.Of(description));

        planFeature.MarkAsCreated(createdAt);

        return planFeature;
    }
}