namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Specifies the billing lifecycle status of a subscription.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>
    ///     The subscription is within its initial trial period and has not yet been billed.
    /// </summary>
    Trialing,

    /// <summary>
    ///     The subscription is active and in good standing.
    /// </summary>
    Active,

    /// <summary>
    ///     The most recent payment attempt failed and the subscription is awaiting resolution.
    /// </summary>
    PastDue,

    /// <summary>
    ///     The subscription has been cancelled. This is a terminal state.
    /// </summary>
    Canceled,

    /// <summary>
    ///     Payment could not be collected after repeated attempts.
    /// </summary>
    Unpaid
}