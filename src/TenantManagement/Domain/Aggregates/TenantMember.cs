using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.Enums;
using HauteCouture.TenantManagement.Domain.Exceptions;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Aggregates;

/// <summary>
///     Represents a user's membership within a tenant.
/// </summary>
public sealed class TenantMember : AuditableEntity<MemberId>
{
    /// <summary>
    ///     The identifier of the tenant this membership belongs to.
    /// </summary>
    public TenantId TenantId { get; private set; }

    /// <summary>
    ///     The identifier of the underlying platform user.
    /// </summary>
    public MemberUserId UserId { get; private set; }

    /// <summary>
    ///     The role currently assigned to the member within the tenant.
    /// </summary>
    public MemberRole Role { get; private set; }

    /// <summary>
    ///     The current membership status.
    /// </summary>
    public MemberStatus Status { get; private set; }

    /// <summary>
    ///     The timestamp at which the member was removed from the tenant, if removed.
    /// </summary>
    public MemberRemovalDate? RemovedAt { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private TenantMember()
    {
    }

    private TenantMember(
        MemberId memberId,
        TenantId tenantId,
        MemberUserId userId,
        MemberRole role,
        MemberStatus status)
    {
        Id = memberId;
        TenantId = tenantId;
        UserId = userId;
        Role = role;
        Status = status;
    }

    /// <summary>
    ///     Creates the founding <see cref="TenantMember"/> for a tenant, assigned the
    ///     <see cref="MemberRole.Owner"/> role.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="tenantId">The identifier of the owning tenant.</param>
    /// <param name="userId">The identifier of the underlying platform user.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="TenantMember"/>, with the <see cref="MemberRole.Owner"/> role.</returns>
    public static TenantMember CreateOwner(
        Guid id,
        Guid tenantId,
        Guid userId,
        DateTimeOffset createdAt)
    {
        var tenantMember = new TenantMember(
            MemberId.Of(id),
            TenantId.Of(tenantId),
            MemberUserId.Of(userId),
            MemberRole.Owner,
            MemberStatus.Active);

        tenantMember.MarkAsCreated(createdAt);

        return tenantMember;
    }

    /// <summary>
    ///     Creates a <see cref="TenantMember"/> resulting from an accepted invitation.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="tenantId">The identifier of the owning tenant.</param>
    /// <param name="userId">The identifier of the underlying platform user.</param>
    /// <param name="role">The role to assign to the new member.</param>
    /// <param name="createdAt">The timestamp to record as the creation time.</param>
    /// <returns>The created <see cref="TenantMember"/>.</returns>
    public static TenantMember CreateFromInvitation(
        Guid id,
        Guid tenantId,
        Guid userId,
        MemberRole role,
        DateTimeOffset createdAt)
    {
        if (role == MemberRole.Owner)
        {
            throw new TenantMemberException("An invited member cannot be assigned the owner role.");
        }

        var tenantMember = new TenantMember(
           MemberId.Of(id),
           TenantId.Of(tenantId),
           MemberUserId.Of(userId),
           role,
           MemberStatus.Active);

        tenantMember.MarkAsCreated(createdAt);

        return tenantMember;
    }

    /// <summary>
    ///     Changes the member's role to <paramref name="newRole"/>.
    /// </summary>
    /// <param name="newRole">The new role to assign.</param>
    /// <param name="updatedAt">The timestamp to record as the update time.</param>
    public void UpdateRole(
        MemberRole newRole,
        DateTimeOffset updatedAt)
    {
        if (Status == MemberStatus.Removed)
        {
            throw new TenantMemberException("Cannot change the role of a removed member.");
        }
        if (Role == newRole)
        {
            throw new TenantMemberException($"Member already has the '{newRole}' role.");
        }
        if (Role == MemberRole.Owner && newRole != MemberRole.Owner)
        {
            throw new TenantMemberException("The owner cannot change their role.");
        }

        Role = newRole;
        MarkAsUpdated(updatedAt);
    }

    /// <summary>
    ///     Promotes the member to the <see cref="MemberRole.Owner"/> role.
    /// </summary>
    /// <param name="promotedAt">The timestamp to record as the promotion time.</param>
    public void PromoteToOwner(DateTimeOffset promotedAt)
    {
        if (Status == MemberStatus.Removed)
        {
            throw new TenantMemberException("Cannot promote a removed member to owner.");
        }
        if (Role == MemberRole.Owner)
        {
            throw new TenantMemberException("Member is already an owner.");
        }

        Role = MemberRole.Owner;
        MarkAsUpdated(promotedAt);
    }

    /// <summary>
    ///     Demotes an owner to a non-owner role.
    /// </summary>
    /// <param name="newRole">The role to assign after demotion.</param>
    /// <param name="demotedAt">The timestamp to record as the demotion time.</param>
    public void DemoteFromOwner(
        MemberRole newRole,
        DateTimeOffset demotedAt)
    {
        if (Role != MemberRole.Owner)
        {
            throw new TenantMemberException("Only an owner can be demoted.");
        }
        if (newRole == MemberRole.Owner)
        {
            throw new TenantMemberException("New role must be different from owner.");
        }
        if (Status == MemberStatus.Removed)
        {
            throw new TenantMemberException("Cannot change the role of a removed member.");
        }

        Role = newRole;
        MarkAsUpdated(demotedAt);
    }

    /// <summary>
    ///     Removes the member from the tenant. This is a terminal state.
    /// </summary>
    /// <param name="removedAt">The timestamp to record as the removal time.</param>
    public void Remove(DateTimeOffset removedAt)
    {
        if (Status == MemberStatus.Removed)
        {
            throw new TenantMemberException("Member has already been removed.");
        }
        if (removedAt < CreatedAt)
        {
            throw new TenantMemberException("Member removal date cannot be earlier than the member creation date.");
        }
        if (Role == MemberRole.Owner)
        {
            throw new TenantMemberException("The owner cannot be removed from the tenant.");
        }

        Status = MemberStatus.Removed;
        RemovedAt = MemberRemovalDate.Of(removedAt);
        MarkAsUpdated(removedAt);
    }
}