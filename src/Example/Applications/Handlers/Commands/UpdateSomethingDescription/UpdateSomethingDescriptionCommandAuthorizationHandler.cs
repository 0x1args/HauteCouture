using HauteCouture.Shared.Common.Authorization;
using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Example.Applications.Handlers.Commands.UpdateSomethingDescription;

/// <summary>
///     Authorizes <see cref="UpdateSomethingDescriptionCommand"/> execution.
/// </summary>
public sealed class UpdateSomethingDescriptionCommandAuthorizationHandler(
    ICurrentUserSession currentUser)
    : BaseRoleAuthorizationHandler<UpdateSomethingDescriptionCommand>(currentUser)
{
    /// <inheritdoc />
    public override Task AuthorizeAsync(
        UpdateSomethingDescriptionCommand command,
        CancellationToken cancellationToken)
    {
        RequireRole(
            "Update something",
            UserRoleDefinitions.PlatformAdministrator);

        return Task.CompletedTask;
    }
}