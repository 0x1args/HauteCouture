using HauteCouture.Shared.Common.Authorization;
using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Example.Applications.Handlers.Queries.GetSomething;

/// <summary>
///     Authorizes <see cref="GetSomethingQuery"/> execution.
/// </summary>
public sealed class GetSomethingQueryAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<GetSomethingQuery>(currentUser)
{
    /// <inheritdoc />
    public override Task AuthorizeAsync(
        GetSomethingQuery query,
        CancellationToken cancellationToken)
    {
        RequireRole(
            "Get something",
            UserRoleDefinitions.PlatformAdministrator,
            UserRoleDefinitions.PlatformSupport,
            UserRoleDefinitions.StaffMember);

        return Task.CompletedTask;
    }
}