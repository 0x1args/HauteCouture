using HauteCouture.Shared.Domain;
using HauteCouture.TenantManagement.Domain.Enums;
using HauteCouture.TenantManagement.Domain.Exceptions;
using HauteCouture.TenantManagement.Domain.ValueObjects;

namespace HauteCouture.TenantManagement.Domain.Aggregates;

/// <summary>
///     Represents a tenant's subscription to a plan.
/// </summary>
public sealed class Subscription : AuditableEntity<SubscriptionId>
{
    /// <summary>
    ///     The identifier of the subscribing tenant.
    /// </summary>
    public TenantId TenantId { get; private set; }

    /// <summary>
    ///     The identifier of the subscribed plan.
    /// </summary>
    public PlanId PlanId { get; private set; }

    /// <summary>
    ///     The current status of the subscription.
    /// </summary>
    public SubscriptionStatus Status { get; private set; }

    /// <summary>
    ///     The billing interval used for the current period.
    /// </summary>
    public BillingInterval Interval { get; private set; }

    /// <summary>
    ///     The start of the current billing period.
    /// </summary>
    public SubscriptionStartDate CurrentPeriodStart { get; private set; }

    /// <summary>
    ///     The end of the current billing period.
    /// </summary>
    public SubscriptionEndDate CurrentPeriodEnd { get; private set; }

    /// <summary>
    ///     The timestamp at which the trial period ends, if the subscription is trialing.
    /// </summary>
    public SubscriptionTrialEndDate? TrialEndsAt { get; private set; }

    /// <summary>
    ///     The timestamp at which the subscription was cancelled, if cancelled.
    /// </summary>
    public SubscriptionCancellationDate? CancelledAt { get; private set; }

    /// <summary>
    ///     Whether the subscription is scheduled to cancel at the end of the current period.
    /// </summary>
    public bool CancelAtPeriodEnd { get; private set; }

    /// <summary>
    ///     The identifier of the corresponding customer record in the external billing provider.
    /// </summary>
    public ExternalCustomerId? ExternalProviderCustomerId { get; private set; }

    /// <summary>
    ///     The identifier of the corresponding subscription record in the external billing provider.
    /// </summary>
    public ExternalSubscriptionId? ExternalProviderSubscriptionId { get; private set; }

    /// <summary>
    ///     Reserved for EF Core materialization.
    /// </summary>
    private Subscription()
    {
    }

    private Subscription(
        SubscriptionId id,
        TenantId tenantId,
        PlanId planId,
        SubscriptionStatus status,
        BillingInterval interval,
        SubscriptionStartDate currentPeriodStart,
        SubscriptionEndDate currentPeriodEnd,
        SubscriptionTrialEndDate? trialEndsAt,
        bool cancelAtPeriodEnd)
    {
        Id = id;
        TenantId = tenantId;
        PlanId = planId;
        Status = status;
        Interval = interval;
        CurrentPeriodStart = currentPeriodStart;
        CurrentPeriodEnd = currentPeriodEnd;
        TrialEndsAt = trialEndsAt;
        CancelAtPeriodEnd = cancelAtPeriodEnd;
    }

    /// <summary>
    ///     Creates a new <see cref="Subscription"/> in the <see cref="SubscriptionStatus.Trialing"/> status.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <param name="tenantId">The identifier of the subscribing tenant.</param>
    /// <param name="planId">The identifier of the subscribed plan.</param>
    /// <param name="trialDuration">The duration of the trial period.</param>
    /// <param name="createdAt">The timestamp to record as the creation time, and the trial's start.</param>
    /// <returns>The created <see cref="Subscription"/>, in the <see cref="SubscriptionStatus.Trialing"/> status.</returns>
    public static Subscription CreateTrial(
        Guid id,
        Guid tenantId,
        Guid planId,
        TimeSpan trialDuration,
        DateTimeOffset createdAt)
    {
        if (trialDuration <= TimeSpan.Zero)
        {
            throw new SubscriptionException("Trial duration must be greater than zero.");
        }

        var subscription = new Subscription(
            SubscriptionId.Of(id),
            TenantId.Of(tenantId),
            PlanId.Of(planId),
            SubscriptionStatus.Trialing,
            BillingInterval.Monthly,
            SubscriptionStartDate.Of(createdAt),
            SubscriptionEndDate.Of(createdAt.Add(trialDuration)),
            SubscriptionTrialEndDate.Of(createdAt.Add(trialDuration)),
            false);

        subscription.MarkAsCreated(createdAt);

        return subscription;
    }

    /// <summary>
    ///     Activates the subscription following successful billing setup.
    /// </summary>
    /// <param name="providerCustomerId">The customer identifier assigned by the external provider.</param>
    /// <param name="providerSubscriptionId">The subscription identifier assigned by the external provider.</param>
    /// <param name="periodEnd">The end of the newly activated billing period.</param>
    /// <param name="activatedAt">The timestamp to record as the activation time.</param>
    public void Activate(
        string providerCustomerId,
        string providerSubscriptionId,
        DateTimeOffset periodEnd,
        DateTimeOffset activatedAt)
    {
        EnsureStatusTransitionAllowedTo(SubscriptionStatus.Active);

        if (activatedAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Subscription activation date cannot be earlier than the subscription creation date.");
        }
        if (periodEnd <= CurrentPeriodStart.Value)
        {
            throw new SubscriptionException(
                "Subscription period end must be later than the current period start.");
        }

        Status = SubscriptionStatus.Active;
        ExternalProviderCustomerId = ExternalCustomerId.Of(providerCustomerId);
        ExternalProviderSubscriptionId = ExternalSubscriptionId.Of(providerSubscriptionId);
        CurrentPeriodEnd = SubscriptionEndDate.Of(periodEnd);
        TrialEndsAt = null;
        MarkAsUpdated(activatedAt);
    }

    /// <summary>
    ///     Records a successful payment, rolling the subscription forward into a new billing period.
    /// </summary>
    /// <param name="newPeriodEnd">The end of the new billing period.</param>
    /// <param name="recordedAt">The timestamp to record as the payment time.</param>
    public void RecordSuccessfulPayment(
        DateTimeOffset newPeriodEnd,
        DateTimeOffset recordedAt)
    {
        if (Status == SubscriptionStatus.Canceled)
        {
            throw new SubscriptionException(
                "Cannot record a payment for a canceled subscription.");
        }
        if (recordedAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Payment date cannot be earlier than the subscription creation date.");
        }
        if (newPeriodEnd <= CurrentPeriodEnd.Value)
        {
            throw new SubscriptionException(
                "The new billing period must end after the current billing period.");
        }

        Status = SubscriptionStatus.Active;
        CurrentPeriodStart = SubscriptionStartDate.Of(CurrentPeriodEnd.Value);
        CurrentPeriodEnd = SubscriptionEndDate.Of(newPeriodEnd);
        MarkAsUpdated(recordedAt);
    }

    /// <summary>
    ///     Marks the subscription as past due, typically following a failed payment attempt.
    /// </summary>
    /// <param name="markedAt">The timestamp to record as the past-due time.</param>
    public void MarkPastDue(DateTimeOffset markedAt)
    {
        EnsureStatusTransitionAllowedTo(SubscriptionStatus.PastDue);

        if (markedAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Past due date cannot be earlier than the subscription creation date.");
        }

        Status = SubscriptionStatus.PastDue;
        MarkAsUpdated(markedAt);
    }

    /// <summary>
    ///     Marks the subscription as unpaid, typically after repeated failed payment attempts.
    /// </summary>
    /// <param name="markedAt">The timestamp to record as the unpaid time.</param>
    public void MarkUnpaid(DateTimeOffset markedAt)
    {
        EnsureStatusTransitionAllowedTo(SubscriptionStatus.Unpaid);

        if (markedAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Unpaid date cannot be earlier than the subscription creation date.");
        }

        Status = SubscriptionStatus.Unpaid;
        MarkAsUpdated(markedAt);
    }

    /// <summary>
    ///     Changes the subscription's plan.
    /// </summary>
    /// <param name="newPlanId">The identifier of the new plan.</param>
    /// <param name="changedAt">The timestamp to record as the change time.</param>
    public void ChangePlan(
        Guid newPlanId,
        DateTimeOffset changedAt)
    {
        if (Status == SubscriptionStatus.Canceled)
        {
            throw new SubscriptionException("Cannot change the plan of a canceled subscription.");
        }
        if (changedAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Plan change date cannot be earlier than the subscription creation date.");
        }

        var planId = PlanId.Of(newPlanId);

        if (PlanId == planId)
        {
            throw new SubscriptionException("The subscription is already assigned to the specified plan.");
        }

        PlanId = planId;
        MarkAsUpdated(changedAt);
    }

    /// <summary>
    ///     Schedules the subscription to cancel automatically at the end of the current period.
    /// </summary>
    /// <param name="scheduledAt">The timestamp to record as the scheduling time.</param>
    public void ScheduleCancellation(DateTimeOffset scheduledAt)
    {
        if (Status == SubscriptionStatus.Canceled)
        {
            throw new SubscriptionException("Subscription has already been canceled.");
        }
        if (CancelAtPeriodEnd)
        {
            throw new SubscriptionException("Subscription cancellation has already been scheduled.");
        }
        if (scheduledAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Cancellation scheduling date cannot be earlier than the subscription creation date.");
        }

        CancelAtPeriodEnd = true;
        MarkAsUpdated(scheduledAt);
    }

    /// <summary>
    ///     Reverses a previously scheduled end-of-period cancellation.
    /// </summary>
    /// <param name="resumedAt">The timestamp to record as the resume time.</param>
    public void ResumeCancellation(DateTimeOffset resumedAt)
    {
        if (!CancelAtPeriodEnd)
        {
            throw new SubscriptionException("Subscription cancellation has not been scheduled.");
        }
        if (Status == SubscriptionStatus.Canceled)
        {
            throw new SubscriptionException("Cannot resume a canceled subscription.");
        }
        if (resumedAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Cancellation resume date cannot be earlier than the subscription creation date.");
        }

        CancelAtPeriodEnd = false;
        MarkAsUpdated(resumedAt);
    }

    /// <summary>
    ///     Cancels the subscription immediately.
    /// </summary>
    /// <param name="cancelledAt">The timestamp to record as the cancellation time.</param>
    public void Cancel(DateTimeOffset cancelledAt)
    {
        EnsureStatusTransitionAllowedTo(SubscriptionStatus.Canceled);

        if (cancelledAt < CreatedAt)
        {
            throw new SubscriptionException(
                "Subscription cancellation date cannot be earlier than the subscription creation date.");
        }

        Status = SubscriptionStatus.Canceled;
        CancelledAt = SubscriptionCancellationDate.Of(cancelledAt);
        CancelAtPeriodEnd = false;
        MarkAsUpdated(cancelledAt);
    }

    private void EnsureStatusTransitionAllowedTo(
        SubscriptionStatus targetStatus)
    {
        var allowed = (Status, targetStatus) switch
        {
            (SubscriptionStatus.Trialing, SubscriptionStatus.Active) => true,
            (SubscriptionStatus.Trialing, SubscriptionStatus.Canceled) => true,

            (SubscriptionStatus.Active, SubscriptionStatus.PastDue) => true,
            (SubscriptionStatus.Active, SubscriptionStatus.Canceled) => true,

            (SubscriptionStatus.PastDue, SubscriptionStatus.Active) => true,
            (SubscriptionStatus.PastDue, SubscriptionStatus.Unpaid) => true,
            (SubscriptionStatus.PastDue, SubscriptionStatus.Canceled) => true,

            (SubscriptionStatus.Unpaid, SubscriptionStatus.Active) => true,
            (SubscriptionStatus.Unpaid, SubscriptionStatus.Canceled) => true,

            _ => false
        };

        if (!allowed)
        {
            throw new SubscriptionException(
                $"Subscription status transition from '{Status}' to '{targetStatus}' is not allowed.");
        }
    }
}