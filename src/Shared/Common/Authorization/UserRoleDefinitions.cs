namespace HauteCouture.Shared.Common.Authorization;

/// <summary>
///     Provides well-known role definitions used for Keycloak role provisioning and display purposes.
/// </summary>
public sealed class UserRoleDefinitions
{
    /// <summary>Full administrative access to the entire platform.</summary>
    public static readonly UserRoleDefinitions PlatformAdministrator = new()
    {
        Id = nameof(UserRole.PlatformAdministrator),
        Name = "Platform Administrator"
    };

    /// <summary>Read-only access to tenant data for customer support purposes.</summary>
    public static readonly UserRoleDefinitions PlatformSupport = new()
    {
        Id = nameof(UserRole.PlatformSupport),
        Name = "Platform Support"
    };

    /// <summary>Full access to a single tenant, including billing and subscription management.</summary>
    public static readonly UserRoleDefinitions TenantOwner = new()
    {
        Id = nameof(UserRole.TenantOwner),
        Name = "Tenant Owner"
    };

    /// <summary>Administrative access within a single tenant: catalog, staff, schedules, reports.</summary>
    public static readonly UserRoleDefinitions TenantAdministrator = new()
    {
        Id = nameof(UserRole.TenantAdministrator),
        Name = "Tenant Administrator"
    };

    /// <summary>Operational access within a single tenant: bookings, orders, clients.</summary>
    public static readonly UserRoleDefinitions TenantManager = new()
    {
        Id = nameof(UserRole.TenantManager),
        Name = "Tenant Manager"
    };

    /// <summary>Access scoped to the staff member's own schedule and assigned bookings.</summary>
    public static readonly UserRoleDefinitions StaffMember = new()
    {
        Id = nameof(UserRole.StaffMember),
        Name = "Staff Member"
    };

    /// <summary>End-user access: own bookings, orders, and payment history.</summary>
    public static readonly UserRoleDefinitions Client = new()
    {
        Id = nameof(UserRole.Client),
        Name = "Client"
    };

    /// <summary>
    ///     Keycloak role name used as the role identifier during provisioning and JWT claim mapping.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>Human-readable display name shown in the admin UI and audit logs.</summary>
    public string? Name { get; set; }

    /// <inheritdoc />
    public override string ToString() => Name ?? Id ?? base.ToString()!;
}