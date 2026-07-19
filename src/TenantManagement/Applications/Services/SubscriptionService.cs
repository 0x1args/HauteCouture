using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Applications.Services.Abstractions;
using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services;

public sealed class SubscriptionService : ISubscriptionService
{
    public Task ActivateAsync(string providerSubscriptionId, string providerCustomerId, DateTimeOffset periodEnd, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task CancelAsync(string providerSubscriptionId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task ChangePlanAsync(Guid tenantId, Guid newPlanid, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<Guid> CreateTrialAsync(Guid tenantId, Guid planId, TimeSpan trialDuration, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<OffsetPagedList<SubscriptionResponse>> GetSubscriptionsPageAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task MarkPastDueAsync(string providerSubscriptionId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task MarkUnpaidAsync(string providerSubscriptionId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task RecordSuccessfulPaymentAsync(string providerSubscriptionId, DateTimeOffset newPeriodEnd, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task ResumeCancellationAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task ScheduleCancellationAsync(Guid tenantId, CancellationToken cancellationToken) => throw new NotImplementedException();
}