using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.Entities;
using HauteCouture.TenantManagement.Domain.Enums;
using HauteCouture.TenantManagement.Domain.Exceptions;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Aggregates;

/// <summary>
///     Represents a tenant organization provisioned on the platform.
/// </summary>
public sealed class Tenant : AuditableEntity<TenantId>
{
    /// <summary>
    ///     The tenant's display name.
    /// </summary>
    public TenantName Name { get; private set; }

    /// <summary>
    ///     The unique URL-safe slug identifying the tenant.
    /// </summary>
    public TenantSlug Slug { get; private set; }

    /// <summary>
    ///     The current lifecycle status of the tenant.
    /// </summary>
    public TenantStatus Status { get; private set; }

    private readonly List<TenantDomain> _domains = [];

    /// <summary>
    ///     The custom domains associated with the tenant.
    /// </summary>
    public IReadOnlyCollection<TenantDomain> Domains => _domains.AsReadOnly();

    /// <summary>
    ///     The tenant's configurable settings.
    /// </summary>
    public TenantSettings Settings { get; private set; } = null!;

    /// <summary>
    ///     The timestamp at which the tenant was suspended, if currently suspended.
    /// </summary>
    public TenantSuspensionDate? SuspendedAt { get; private set; }

    /// <summary>
    ///     The reason recorded for the tenant's suspension, if currently suspended.
    /// </summary>
    public TenantSuspensionReason? SuspensionReason { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private Tenant()
    {
    }

    private Tenant(
        TenantId id,
        TenantName name,
        TenantSlug slug,
        TenantStatus status,
        TenantSettings settings)
    {
        Id = id;
        Name = name;
        Slug = slug;
        Status = status;
        Settings = settings;
    }

    /// <summary>
    ///     Creates a new <see cref="Tenant"/> pending provisioning, with default settings.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The tenant's display name.</param>
    /// <param name="slug">The unique URL-safe slug.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="Tenant"/>, in the <see cref="TenantStatus.PendingProvisioning"/> status.</returns>
    public static Tenant Create(
        Guid id,
        string name,
        string slug,
        DateTimeOffset createdAt)
    {
        var tenant = new Tenant(
            TenantId.Of(id),
            TenantName.Of(name),
            TenantSlug.Of(slug),
            TenantStatus.PendingProvisioning,
            TenantSettings.Default());

        tenant.MarkAsCreated(createdAt);

        return tenant;
    }

    /// <summary>
    ///     Activates the tenant, transitioning it out of pending provisioning or suspension.
    /// </summary>
    /// <param name="activatedAt">The timestamp to record as the activation time.</param>
    public void Activate(DateTimeOffset activatedAt)
    {
        EnsureStatusTransitionAllowedTo(TenantStatus.Active);

        if (activatedAt < CreatedAt)
        {
            throw new TenantException(
                "Tenant activation date cannot be earlier than the tenant creation date.");
        }

        Status = TenantStatus.Active;
        MarkAsUpdated(activatedAt);
    }

    /// <summary>
    ///     Suspends the tenant for the given reason.
    /// </summary>
    /// <param name="reason">The reason for the suspension.</param>
    /// <param name="suspendedAt">The timestamp to record as the suspension time.</param>
    public void Suspend(
        string reason,
        DateTimeOffset suspendedAt)
    {
        EnsureStatusTransitionAllowedTo(TenantStatus.Suspended);

        if (suspendedAt < CreatedAt)
        {
            throw new TenantException(
                "Tenant suspension date cannot be earlier than the tenant creation date.");
        }

        Status = TenantStatus.Suspended;
        SuspensionReason = TenantSuspensionReason.Of(reason);
        SuspendedAt = TenantSuspensionDate.Of(suspendedAt);
        MarkAsUpdated(suspendedAt);
    }

    /// <summary>
    ///     Reactivates a suspended tenant, clearing the suspension details.
    /// </summary>
    /// <param name="reactivatedAt">The timestamp to record as the reactivation time.</param>
    public void Reactivate(DateTimeOffset reactivatedAt)
    {
        EnsureStatusTransitionAllowedTo(TenantStatus.Active);

        if (reactivatedAt < CreatedAt)
        {
            throw new TenantException(
                "Tenant reactivation date cannot be earlier than the tenant creation date.");
        }

        Status = TenantStatus.Active;
        SuspensionReason = null;
        SuspendedAt = null;
        MarkAsUpdated(reactivatedAt);
    }

    /// <summary>
    ///     Permanently deactivates the tenant. This is a terminal state.
    /// </summary>
    /// <param name="deactivatedAt">The timestamp to record as the deactivation time.</param>
    public void Deactivate(DateTimeOffset deactivatedAt)
    {
        EnsureStatusTransitionAllowedTo(TenantStatus.Deactivated);

        if (deactivatedAt < CreatedAt)
        {
            throw new TenantException(
                "Tenant deactivation date cannot be earlier than the tenant creation date.");
        }

        Status = TenantStatus.Deactivated;
        SuspendedAt = null;
        SuspensionReason = null;
        MarkAsUpdated(deactivatedAt);
    }

    /// <summary>
    ///     Adds a custom domain to the tenant, pending verification.
    /// </summary>
    /// <param name="domainId">The unique identifier for the domain.</param>
    /// <param name="domainName">The domain name to add.</param>
    /// <param name="domainAddedAt">The timestamp to record as the addition time.</param>
    public void AddCustomDomain(
        Guid domainId,
        string domainName,
        DateTimeOffset domainAddedAt)
    {
        if (Status == TenantStatus.Deactivated)
        {
            throw new TenantException(
                "Cannot add custom domains to a deactivated tenant.");
        }
        if (domainAddedAt < CreatedAt)
        {
            throw new TenantException(
                "Custom domain addition date cannot be earlier than the tenant creation date.");
        }

        var name = DomainName.Of(domainName);

        if (_domains.Any(domain => domain.Name == name))
        {
            throw new TenantException($"Custom domain '{name.Value}' has already been added.");
        }

        var tenantDomain = TenantDomain.CreatePendingVerification(
            domainId,
            domainName,
            domainAddedAt);

        _domains.Add(tenantDomain);
        MarkAsUpdated(domainAddedAt);
    }

    /// <summary>
    ///     Marks an existing custom domain as verified.
    /// </summary>
    /// <param name="domainName">The domain name to verify.</param>
    /// <param name="verifiedAt">The timestamp to record as the verification time.</param>
    public void VerifyCustomDomain(
        string domainName,
        DateTimeOffset verifiedAt)
    {
        if (Status == TenantStatus.Deactivated)
        {
            throw new TenantException("Cannot verify custom domains for a deactivated tenant.");
        }

        var name = DomainName.Of(domainName);

        var tenantDomain = _domains
            .SingleOrDefault(d => d.Name == name)
            ?? throw new TenantException($"Custom domain '{name.Value}' was not found.");

        tenantDomain.MarkVerified(verifiedAt);
        MarkAsUpdated(verifiedAt);
    }

    /// <summary>
    ///     Replaces the tenant's settings. No-op if the new settings are unchanged.
    /// </summary>
    /// <param name="settings">The new settings.</param>
    /// <param name="updatedAt">The timestamp to record as the update time.</param>
    public void UpdateSettings(
        TenantSettings settings,
        DateTimeOffset updatedAt)
    {
        if (Status == TenantStatus.Deactivated)
        {
            throw new TenantException("Cannot update settings of a deactivated tenant.");
        }
        if (Settings == settings)
        {
            return;
        }

        Settings = settings;
        MarkAsUpdated(updatedAt);
    }

    private void EnsureStatusTransitionAllowedTo(TenantStatus targetStatus)
    {
        var allowed = (Status, targetStatus) switch
        {
            (TenantStatus.PendingProvisioning, TenantStatus.Active) => true,
            (TenantStatus.Active, TenantStatus.Suspended) => true,
            (TenantStatus.Suspended, TenantStatus.Active) => true,
            (_, TenantStatus.Deactivated) => Status != TenantStatus.Deactivated,
            _ => false
        };

        if (!allowed)
        {
            throw new TenantException(
                $"Tenant status transition from '{Status}' to '{targetStatus}' is not allowed.");
        }
    }
}