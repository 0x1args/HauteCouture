using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Applications.Services.Abstractions;
using HauteCouture.TenantManagement.Contracts.Responses;
using HauteCouture.TenantManagement.Domain.Enums;

namespace HauteCouture.TenantManagement.Applications.Services;

public sealed class TenantMemberService : ITenantMemberService
{
    public Task<int> CountActiveMembersAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Guid> CreateFromInvitationAsync(Guid tenantid, Guid userId, MemberRole role, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Guid> CreateOwnerAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<TenantMemberResponse>> GetMembersByUserIdAsync(Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<TenantMemberResponse>> GetMembersPageAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<TenantMemberResponse>> GetMembersPageByUserIdAsync(Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<bool> IsActiveMemberAsync(Guid tenantId, Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task RemoveAsync(Guid tenantId, Guid memberId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task TransferOwnershipAsync(Guid ownerId, Guid newOwnerId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task UpdateRoleAsync(Guid tenantId, Guid memberId, MemberRole newMemberRole, CancellationToken cancellationToken) => throw new NotImplementedException();
}