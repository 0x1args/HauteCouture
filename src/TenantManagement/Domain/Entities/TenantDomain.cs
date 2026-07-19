using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.Enums;
using HauteCouture.TenantManagement.Domain.Exceptions;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Entities;

/// <summary>
///     Represents a custom domain associated with a <c>Tenant</c> and its verification lifecycle.
/// </summary>
public sealed class TenantDomain : AuditableEntity<DomainId>
{
    /// <summary>
    ///     The domain name.
    /// </summary>
    public DomainName Name { get; private set; }

    /// <summary>
    ///     The current verification status of the domain.
    /// </summary>
    public DomainVerificationStatus Status { get; private set; }

    /// <summary>
    ///     The token used to prove ownership of the domain.
    /// </summary>
    public DomainVerificationToken VerificationToken { get; private set; }

    /// <summary>
    ///     The timestamp at which the domain was verified, if verified.
    /// </summary>
    public DomainVerificationDate? VerifiedAt { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private TenantDomain()
    {
    }

    private TenantDomain(
        DomainId id,
        DomainName name,
        DomainVerificationStatus status,
        DomainVerificationToken verificationToken)
    {
        Id = id;
        Name = name;
        Status = status;
        VerificationToken = verificationToken;
    }

    /// <summary>
    ///     Creates a new <see cref="TenantDomain"/> with a freshly generated verification token.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="name">The domain name.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="TenantDomain"/>, in the <see cref="DomainVerificationStatus.Pending"/> status.</returns>
    public static TenantDomain CreatePendingVerification(
        Guid id,
        string name,
        DateTimeOffset createdAt)
    {
        var tenantDomain = new TenantDomain(
            DomainId.Of(id),
            DomainName.Of(name),
            DomainVerificationStatus.Pending,
            DomainVerificationToken.Generate());

        tenantDomain.MarkAsCreated(createdAt);

        return tenantDomain;
    }

    /// <summary>
    ///     Marks the domain as verified.
    /// </summary>
    /// <param name="verifiedAt">The timestamp to record as the verification time.</param>
    public void MarkVerified(DateTimeOffset verifiedAt)
    {
        EnsureTransitionAllowedTo(DomainVerificationStatus.Verified);

        if (verifiedAt < CreatedAt)
        {
            throw new TenantDomainException(
                "Custom domain verification date cannot be earlier than the domain creation date.");
        }

        Status = DomainVerificationStatus.Verified;
        VerifiedAt = DomainVerificationDate.Of(verifiedAt);
        MarkAsUpdated(verifiedAt);
    }

    /// <summary>
    ///     Marks the domain's verification attempt as failed.
    /// </summary>
    /// <param name="failedAt">The timestamp to record as the failure time.</param>
    public void MarkFailed(DateTimeOffset failedAt)
    {
        EnsureTransitionAllowedTo(DomainVerificationStatus.Failed);

        if (failedAt < CreatedAt)
        {
            throw new TenantDomainException(
                "Domain verification failure date cannot be earlier than the domain creation date.");
        }

        Status = DomainVerificationStatus.Failed;
        VerifiedAt = null;
        MarkAsUpdated(failedAt);
    }

    private void EnsureTransitionAllowedTo(DomainVerificationStatus targetStatus)
    {
        var allowed = (Status, targetStatus) switch
        {
            (DomainVerificationStatus.Pending, DomainVerificationStatus.Verified) => true,
            (DomainVerificationStatus.Pending, DomainVerificationStatus.Failed) => true,
            (DomainVerificationStatus.Failed, DomainVerificationStatus.Pending) => true,
            _ => false
        };

        if (!allowed)
        {
            throw new TenantDomainException(
                $"Domain verification status transition from '{Status}' to '{targetStatus}' is not allowed.");
        }
    }
}