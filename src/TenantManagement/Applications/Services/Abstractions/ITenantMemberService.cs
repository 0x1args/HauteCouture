using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Contracts.Responses;
using HauteCouture.TenantManagement.Domain.Enums;

namespace HauteCouture.TenantManagement.Applications.Services.Abstractions;

public interface ITenantMemberService
{
    Task<Guid> CreateOwnerAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<Guid> CreateFromInvitationAsync(
        Guid tenantid,
        Guid userId,
        MemberRole role,
        CancellationToken cancellationToken);

    Task UpdateRoleAsync(
        Guid tenantId,
        Guid memberId,
        MemberRole newMemberRole,
        CancellationToken cancellationToken);

    Task TransferOwnershipAsync(
        Guid ownerId,
        Guid newOwnerId,
        CancellationToken cancellationToken);

    Task RemoveAsync(
        Guid tenantId,
        Guid memberId,
        CancellationToken cancellationToken);

    Task<int> CountActiveMembersAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task<bool> IsActiveMemberAsync(
        Guid tenantId,
        Guid userId,
        CancellationToken cancellationToken);

    Task<OffsetPagedList<TenantMemberResponse>> GetMembersPageAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task<OffsetPagedList<TenantMemberResponse>> GetMembersPageByUserIdAsync(
       Guid userId,
       CancellationToken cancellationToken);
}