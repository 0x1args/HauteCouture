namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Specifies the cadence at which a subscription is billed.
/// </summary>
public enum BillingInterval
{
    /// <summary>
    ///     The subscription is billed once every month.
    /// </summary>
    Monthly,

    /// <summary>
    ///     The subscription is billed once every year.
    /// </summary>
    Yearly
}