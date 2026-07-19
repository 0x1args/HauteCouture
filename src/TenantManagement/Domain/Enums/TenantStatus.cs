namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Specifies the lifecycle status of a tenant.
/// </summary>
public enum TenantStatus
{
    /// <summary>
    ///     The tenant has been created but its provisioning has not yet completed.
    /// </summary>
    PendingProvisioning,

    /// <summary>
    ///     The tenant is provisioned and operating normally.
    /// </summary>
    Active,

    /// <summary>
    ///     The tenant's access has been temporarily suspended.
    /// </summary>
    Suspended,

    /// <summary>
    ///     The tenant has been permanently deactivated. This is a terminal state.
    /// </summary>
    Deactivated
}