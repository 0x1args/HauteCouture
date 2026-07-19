using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Contracts.Responses;
using HauteCouture.TenantManagement.Domain.Enums;

namespace HauteCouture.TenantManagement.Applications.Services.Abstractions;

public interface ITenantInvitationService
{
    Task<Guid> CreateAsync(
        Guid tenantId,
        string email,
        MemberRole role,
        Guid invitedByMemberId,
        CancellationToken cancellationToken);

    Task<Guid> AcceptAsync(
        string token,
        Guid userId,
        CancellationToken cancellationToken);

    Task RevokeAsync(
        Guid tenantId,
        Guid invitationId,
        CancellationToken cancellationToken);

    Task<int> ExpirePendingInvitationsAsync(CancellationToken cancellationToken);

    Task<OffsetPagedList<TenantInvitationResponse>> GetPendingInvitationsPageAsync(
        Guid tenantId,
        CancellationToken cancellationToken);
}