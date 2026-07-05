## Authorization

Contains the components required for authorization across the entire system. The primary component is the current user session, which provides detailed information about the authenticated user and can be accessed throughout the application wherever user context is required. This layer also defines the platform's well-known roles.

### Registration

To register the authorization components in DI, add the `Shared.Common.Authorization` package and register an implementation of `ICurrentUserSession` using `AddUserSessions`. The package does not assume a specific authentication provider. Instead, the application is responsible for creating the current session from its authentication mechanism (Keycloak).

```csharp
services.AddUserSessions(serviceProvider =>
{
    return new CurrentUserSession(
        userId,
        tenantId,
        roles,
        sessionId,
        ipAddress,
        userAgent);
});
```

### Roles

The package defines a common `UserRole` enumeration representing all well-known roles supported by the platform.

| Role | Description |
|---|---|
| PlatformAdministrator | Full administrative access to the entire platform. |
| PlatformSupport | Read-only platform support access. |
| TenantOwner | Full administrative access within a tenant, including subscription management. |
| TenantAdministrator | Administrative access to tenant resources excluding billing and subscriptions. |
| TenantManager | Operational management of bookings, staff, and clients. |
| StaffMember | Access limited to the authenticated staff member's own resources. |
| Client | End-user access for tenant customers. |