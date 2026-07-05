using HauteCouture.Shared.CQS.Primitives.Commands;

namespace HauteCouture.Example.Applications.Handlers.Commands.DeleteSomething;

/// <summary>
///     Command to soft-delete an existing <c>Something</c>.
/// </summary>
public sealed record DeleteSomethingCommand(
    Guid SomethingId) 
    : ICommand;