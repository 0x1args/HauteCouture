namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Specifies the role granted to a member within a tenant.
/// </summary>
public enum MemberRole
{
    /// <summary>
    ///     The founding member with full, non-transferable control over the tenant.
    ///     Cannot be assigned through an invitation.
    /// </summary>
    Owner,

    /// <summary>
    ///     A member with broad administrative privileges over the tenant.
    /// </summary>
    Administrator,

    /// <summary>
    ///     A member with elevated privileges over a subset of the tenant's operations.
    /// </summary>
    Manager,

    /// <summary>
    ///     A member with standard, non-administrative access to the tenant.
    /// </summary>
    StaffMember
}