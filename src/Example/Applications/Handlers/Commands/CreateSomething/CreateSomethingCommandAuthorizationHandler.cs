using HauteCouture.Shared.Common.Authorization;
using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Example.Applications.Handlers.Commands.CreateSomething;

/// <summary>
///     Authorizes <see cref="CreateSomethingCommand"/> execution.
/// </summary>
public sealed class CreateSomethingCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<CreateSomethingCommand>(currentUser)
{
    /// <inheritdoc />
    public override Task AuthorizeAsync(
        CreateSomethingCommand command,
        CancellationToken cancellationToken)
    {
        RequireRole(
            "Create something",
            UserRoleDefinitions.PlatformAdministrator);

        return Task.CompletedTask;
    }
}