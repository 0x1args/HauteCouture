using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Applications.Services.Abstractions;
using HauteCouture.TenantManagement.Contracts.Responses;
using HauteCouture.TenantManagement.Domain.Enums;

namespace HauteCouture.TenantManagement.Applications.Services;

public sealed class TenantInvitationService : ITenantInvitationService
{
    public Task<Guid> AcceptAsync(string token, Guid userId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Guid> CreateAsync(Guid tenantId, string email, MemberRole role, Guid invitedByMemberId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<int> ExpirePendingInvitationsAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<TenantInvitationResponse>> GetPendingInvitationsAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<TenantInvitationResponse>> GetPendingInvitationsPageAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task RevokeAsync(Guid tenantId, Guid invitationId, CancellationToken cancellationToken) => throw new NotImplementedException();
}