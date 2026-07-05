namespace HauteCouture.Shared.Common.Authorization;

/// <summary>
///     Defines the roles available across the platform.
/// </summary>
public enum UserRole
{
    /// <summary>
    ///     Full administrative access to the entire platform.
    ///     Can manage all tenants, subscriptions, billing, platform configuration,
    ///     system analytics, and assign platform-level roles.
    ///     Intended for internal platform operators only.
    /// </summary>
    PlatformAdministrator,

    /// <summary>
    ///     Read-only access to tenant data and system state for customer support purposes.
    ///     Cannot modify any tenant or platform data.
    ///     Intended for support team members who need to investigate issues on behalf of tenants.
    /// </summary>
    PlatformSupport,

    /// <summary>
    ///     Full access to a single tenant's resources, including billing, subscription management,
    ///     and all administrative capabilities. Can assign and revoke all tenant-level roles.
    ///     Typically assigned to the business owner who registered the tenant.
    /// </summary>
    TenantOwner,

    /// <summary>
    ///     Administrative access within a single tenant.
    ///     Can manage the service catalog, staff members, schedules, resources,
    ///     and view reports and analytics. Cannot access billing or subscription settings.
    /// </summary>
    TenantAdministrator,

    /// <summary>
    ///     Operational access within a single tenant.
    ///     Can manage bookings, orders, and client interactions.
    ///     Has read access to the service catalog and staff schedules.
    ///     Cannot access billing, subscription, or administrative settings.
    /// </summary>
    TenantManager,

    /// <summary>
    ///     Limited access scoped to the authenticated staff member's own data.
    ///     Can view their assigned bookings, personal schedule, and update
    ///     the status of services they are responsible for.
    ///     Cannot access other staff members' data, billing, or catalog management.
    /// </summary>
    StaffMember,

    /// <summary>
    ///     End-user role assigned to customers of a tenant.
    /// </summary>
    Client,
}