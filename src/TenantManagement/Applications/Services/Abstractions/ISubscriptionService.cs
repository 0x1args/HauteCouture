using HauteCouture.Shared.Common.Pagination;
using HauteCouture.TenantManagement.Contracts.Responses;

namespace HauteCouture.TenantManagement.Applications.Services.Abstractions;

public interface ISubscriptionService
{
    Task<Guid> CreateTrialAsync(
        Guid tenantId,
        Guid planId, 
        TimeSpan trialDuration,
        CancellationToken cancellationToken);

    Task ActivateAsync(
        string providerSubscriptionId,
        string providerCustomerId,
        DateTimeOffset periodEnd,
        CancellationToken cancellationToken);

    Task RecordSuccessfulPaymentAsync(
        string providerSubscriptionId, 
        DateTimeOffset newPeriodEnd, 
        CancellationToken cancellationToken);

    Task MarkPastDueAsync(
        string providerSubscriptionId,
        CancellationToken cancellationToken);

    Task MarkUnpaidAsync(
        string providerSubscriptionId,
        CancellationToken cancellationToken);

    Task ChangePlanAsync(
        Guid tenantId,
        Guid newPlanid,
        CancellationToken cancellationToken);

    Task ScheduleCancellationAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task ResumeCancellationAsync(
        Guid tenantId,
        CancellationToken cancellationToken);

    Task CancelAsync(
        string providerSubscriptionId, 
        CancellationToken cancellationToken);

    Task<OffsetPagedList<SubscriptionResponse>> GetSubscriptionsPageAsync(
        Guid tenantId,
        CancellationToken cancellationToken);
}