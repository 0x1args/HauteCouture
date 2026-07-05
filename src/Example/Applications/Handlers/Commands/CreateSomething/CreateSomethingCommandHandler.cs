using HauteCouture.Example.Applications.AppServices.Abstractions;
using HauteCouture.Shared.CQS.Abstractions.Handlers;

namespace HauteCouture.Example.Applications.Handlers.Commands.CreateSomething;

/// <summary>
///     Handles <see cref="CreateSomethingCommand"/>.
/// </summary>
public sealed class SomethingCommandHandler(
    ISomethingService somethingService) 
    : ICommandHandler<CreateSomethingCommand, Guid>
{
    /// <inheritdoc />
    public async Task<Guid> Handle(
        CreateSomethingCommand command,
        CancellationToken cancellationToken)
    {
        return await somethingService.CreateSomethingAsync(
            command.Request.Name,
            command.Request.Description,
            command.Request.Price,
            command.UserId,
            cancellationToken);
    }
}