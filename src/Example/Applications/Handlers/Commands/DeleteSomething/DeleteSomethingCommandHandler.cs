using HauteCouture.Example.Applications.Services.Abstractions;
using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Example.Applications.Handlers.Commands.DeleteSomething;

/// <summary>
///     Handles <see cref="DeleteSomethingCommand"/>.
/// </summary>
public sealed class DeleteSomethingCommandHandler(
    ISomethingService somethingService)
    : ICommandHandler<DeleteSomethingCommand>
{
    /// <inheritdoc />
    public async Task Handle(
        DeleteSomethingCommand command,
        CancellationToken cancellationToken)
    {
        await somethingService.DeleteSomethingAsync(
            command.SomethingId,
            cancellationToken);
    }
}