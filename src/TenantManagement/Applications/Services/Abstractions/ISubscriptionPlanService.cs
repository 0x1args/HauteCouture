using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services.Abstractions;

public interface ISubscriptionPlanService
{
    Task<Guid> CreateAsync(
       string code, 
       string name, 
       decimal monthlyPrice, 
       decimal yearlyPrice, 
       string currency,
       int maxUsers, 
       int maxDomains,
       int maxStorageInGb,
       int maxApiRequestsPerMonth,
       CancellationToken cancellationToken);

    Task AddFeatureAsync(
        Guid planId,
        string key,
        string description,
        CancellationToken cancellationToken);

    Task ActivateAsync(
        Guid planId,
        CancellationToken cancellationToken);

    Task DeprecateAsync(
        Guid planId, 
        CancellationToken cancellationToken);

    Task<SubscriptionPlanResponse> GetPlanAsync(
        Guid planId,
        CancellationToken cancellationToken);

    Task<SubscriptionPlanResponse> GetPlanByCodeAsync(
        string code,
        CancellationToken cancellationToken);

    Task<OffsetPagedList<SubscriptionPlanResponse>> GetActivePlansPageAsync(CancellationToken cancellationToken);
}