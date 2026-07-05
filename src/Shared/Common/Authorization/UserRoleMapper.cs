namespace HauteCouture.Shared.Common.Authorization;

/// <summary>
///     Extension methods for mapping <see cref="UserRole"/> enum values
///     to their corresponding <see cref="UserRoleDefinitions"/>.
/// </summary>
public static class UserRoleMapper
{
    /// <summary>
    ///     Converts a <see cref="UserRole"/> to its <see cref="UserRoleDefinitions"/> representation.
    /// </summary>
    /// <param name="role">Role to convert.</param>
    /// <returns>
    ///     Matching well-known <see cref="UserRoleDefinitions"/> instance if found;
    ///     otherwise a new instance with <c>Id</c> and <c>Name</c> derived from the enum member name.
    /// </returns>
    public static UserRoleDefinitions ToDefinition(this UserRole role) =>
        role switch
        {
            UserRole.PlatformAdministrator => UserRoleDefinitions.PlatformAdministrator,
            UserRole.PlatformSupport => UserRoleDefinitions.PlatformSupport,
            UserRole.TenantOwner => UserRoleDefinitions.TenantOwner,
            UserRole.TenantAdministrator => UserRoleDefinitions.TenantAdministrator,
            UserRole.TenantManager => UserRoleDefinitions.TenantManager,
            UserRole.StaffMember => UserRoleDefinitions.StaffMember,
            UserRole.Client => UserRoleDefinitions.Client,
            _ => new UserRoleDefinitions { Id = role.ToString(), Name = role.ToString() }
        };
}