using HauteCouture.Shared.Common.Authorization;
using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Example.Applications.Handlers.Commands.DeleteSomething;

/// <summary>
///     Authorizes <see cref="DeleteSomethingCommand"/> execution.
/// </summary>
public sealed class DeleteSomethingCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<DeleteSomethingCommand>(currentUser)
{
    /// <inheritdoc />
    public override Task AuthorizeAsync(
        DeleteSomethingCommand command,
        CancellationToken cancellationToken)
    {
        RequireRole(
            "Delete something",
            UserRoleDefinitions.PlatformAdministrator);

        return Task.CompletedTask;
    }
}