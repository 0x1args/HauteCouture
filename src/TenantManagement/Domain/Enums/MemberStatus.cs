namespace HauteCouture.TenantManagement.Domain.Enums;

/// <summary>
///     Specifies the membership status of a tenant member.
/// </summary>
public enum MemberStatus
{
    /// <summary>
    ///     The member has active access to the tenant.
    /// </summary>
    Active,

    /// <summary>
    ///     The member has been removed from the tenant. This is a terminal state.
    /// </summary>
    Removed
}