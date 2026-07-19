using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Applications.Services.Abstractions;
using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services;

public sealed class SubscriptionPlanService : ISubscriptionPlanService
{
    public Task ActivateAsync(Guid planId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task AddFeatureAsync(Guid planId, string key, string description, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Guid> CreateAsync(string code, string name, decimal monthlyPrice, decimal yearlyPrice, string currency, int maxUsers, int maxDomains, int maxStorageInGb, int maxApiRequestsPerMonth, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task DeprecateAsync(Guid planId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<SubscriptionPlanResponse>> GetActivePlansAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<SubscriptionPlanResponse>> GetActivePlansPageAsync(CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<SubscriptionPlanResponse> GetPlanAsync(Guid planId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<SubscriptionPlanResponse> GetPlanByCodeAsync(string code, CancellationToken cancellationToken) => throw new NotImplementedException();
}