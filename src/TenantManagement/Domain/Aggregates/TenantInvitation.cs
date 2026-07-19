using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.Enums;
using HauteCouture.TenantManagement.Domain.Exceptions;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Aggregates;

/// <summary>
///     Represents an invitation for a prospective user to join a tenant with a proposed role.
/// </summary>
public sealed class TenantInvitation : AuditableEntity<InvitationId>
{
    /// <summary>
    ///     The email address the invitation was sent to.
    /// </summary>
    public InvitationEmailAddress Email { get; private set; }

    /// <summary>
    ///     The role that will be granted if the invitation is accepted.
    /// </summary>
    public MemberRole ProposedRole { get; private set; }

    /// <summary>
    ///     The current status of the invitation.
    /// </summary>
    public InvitationStatus Status { get; private set; }

    /// <summary>
    ///     The unique token used to redeem the invitation.
    /// </summary>
    public InvitationToken Token { get; private set; }

    /// <summary>
    ///     The identifier of the member who issued the invitation.
    /// </summary>
    public InviterMemberId InvitedByMemberId { get; private set; }

    /// <summary>
    ///     The timestamp after which the invitation can no longer be accepted.
    /// </summary>
    public InvitationExpirationDate ExpiresAt { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private TenantInvitation()
    {
    }

    private TenantInvitation(
        InvitationId id,
        InvitationEmailAddress email,
        MemberRole proposedRole,
        InvitationStatus status,
        InvitationToken token,
        InviterMemberId invitedByMemberId,
        InvitationExpirationDate expiresAt)
    {
        Id = id;
        Email = email;
        ProposedRole = proposedRole;
        Status = status;
        Token = token;
        InvitedByMemberId = invitedByMemberId;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    ///     Creates a new pending <see cref="TenantInvitation"/> with a freshly generated token.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="email">The email address to invite.</param>
    /// <param name="proposedRole">The role to grant upon acceptance.</param>
    /// <param name="invitedByMemberId">The identifier of the inviting member.</param>
    /// <param name="validFor">The duration for which the invitation remains valid.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="TenantInvitation"/>, in the <see cref="InvitationStatus.Pending"/> status.</returns>
    public static TenantInvitation Create(
        Guid id,
        string email,
        MemberRole proposedRole,
        Guid invitedByMemberId,
        TimeSpan validFor,
        DateTimeOffset createdAt)
    {
        if (proposedRole == MemberRole.Owner)
        {
            throw new TenantInvitationException("Owner role cannot be assigned through an invitation.");
        }
        if (validFor <= TimeSpan.Zero)
        {
            throw new TenantInvitationException("Invitation validity period must be greater than zero.");
        }

        var tenantInvitation = new TenantInvitation(
            InvitationId.Of(id),
            InvitationEmailAddress.Of(email),
            proposedRole,
            InvitationStatus.Pending,
            InvitationToken.Generate(),
            InviterMemberId.Of(invitedByMemberId),
            InvitationExpirationDate.Of(createdAt.Add(validFor)));

        tenantInvitation.MarkAsCreated(createdAt);

        return tenantInvitation;
    }

    /// <summary>
    ///     Accepts the invitation.
    /// </summary>
    /// <param name="acceptedAt">The timestamp to record as the acceptance time.</param>
    public void Accept(DateTimeOffset acceptedAt)
    {
        EnsureTransitionAllowedTo(InvitationStatus.Accepted);

        if (acceptedAt < CreatedAt)
        {
            throw new TenantInvitationException(
                "Invitation acceptance date cannot be earlier than the invitation creation date.");
        }
        if (acceptedAt > ExpiresAt.Value)
        {
            throw new TenantInvitationException("Invitation has expired.");
        }

        Status = InvitationStatus.Accepted;
        MarkAsUpdated(acceptedAt);
    }

    /// <summary>
    ///     Revokes the invitation, preventing it from being accepted.
    /// </summary>
    /// <param name="revokedAt">The timestamp to record as the revocation time.</param>
    public void Revoke(DateTimeOffset revokedAt)
    {
        EnsureTransitionAllowedTo(InvitationStatus.Revoked);

        if (revokedAt < CreatedAt)
        {
            throw new TenantInvitationException(
                "Invitation revocation date cannot be earlier than the invitation creation date.");
        }

        Status = InvitationStatus.Revoked;
        MarkAsUpdated(revokedAt);
    }

    /// <summary>
    ///     Marks the invitation as expired.
    /// </summary>
    /// <param name="expiredAt">The timestamp to record as the expiration time.</param>
    public void MarkAsExpired(DateTimeOffset expiredAt)
    {
        EnsureTransitionAllowedTo(InvitationStatus.Expired);

        if (expiredAt < CreatedAt)
        {
            throw new TenantInvitationException(
                "Invitation expiration date cannot be earlier than the invitation creation date.");
        }

        Status = InvitationStatus.Expired;
        MarkAsUpdated(expiredAt);
    }

    private void EnsureTransitionAllowedTo(InvitationStatus targetStatus)
    {
        var allowed = (Status, targetStatus) switch
        {
            (InvitationStatus.Pending, InvitationStatus.Accepted) => true,
            (InvitationStatus.Pending, InvitationStatus.Revoked) => true,
            (InvitationStatus.Pending, InvitationStatus.Expired) => true,
            _ => false
        };

        if (!allowed)
        {
            throw new TenantInvitationException(
                $"Invitation status transition from '{Status}' to '{targetStatus}' is not allowed.");
        }
    }
}