using HauteCouture.Shared.Common.Authorization;
using HauteCouture.Shared.Common.Exceptions.Client;
using MediatR;

namespace HauteCouture.Shared.CQS.Abstractions.Handlers;

/// <summary>
///     Base authorization handler that provides role-based access validation.
/// </summary>
/// <typeparam name="TRequest">Type of the request to authorize.</typeparam>
public abstract class BaseRoleAuthorizationHandler<TRequest>(
    ICurrentUserSession currentUser) 
    : IAuthorizationHandler<TRequest>
    where TRequest : IBaseRequest
{
    /// <summary>The current authenticated user's session context.</summary>
    protected ICurrentUserSession CurrentUser { get; } = currentUser;

    /// <inheritdoc/>
    public abstract Task AuthorizeAsync(TRequest request, CancellationToken cancellationToken);

    protected void RequireRole(string operation, params UserRoleDefinitions[] allowedRoles)
    {
        var userRoles = CurrentUser.RoleDefinitions;

        if (userRoles is null || userRoles.Count == 0)
        {
            throw new ForbiddenException(
                BuildAccessDeniedMessage(operation, allowedRoles));
        }

        // Platform administrators have access to all operations.
        if (userRoles.Any(r => string.Equals(r.Id, UserRoleDefinitions.PlatformAdministrator.Id,
            StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        // Check if the user has any of the allowed roles for the operation.
        if (userRoles.Any(r => allowedRoles.Any(a =>
            string.Equals(a.Id, r.Id, StringComparison.OrdinalIgnoreCase))))
        {
            return;
        }   

        throw new ForbiddenException(
            BuildAccessDeniedMessage(operation, allowedRoles));
    }

    /// <summary>
    ///     Builds a human-readable access denied message.
    /// </summary>
    private static string BuildAccessDeniedMessage(
        string operation,
        IEnumerable<UserRoleDefinitions> allowedRoles)
    {
        var roles = string.Join(", ", allowedRoles.Select(r => r.ToString()));
        return $"Access denied for operation '{operation}'. Required roles: {roles}.";
    }
}