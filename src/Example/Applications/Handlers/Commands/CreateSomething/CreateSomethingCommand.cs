using HauteCouture.Example.Contracts.Requests;
using HauteCouture.Shared.CQS.Primitives.Commands;

namespace HauteCouture.Example.Applications.Handlers.Commands.CreateSomething;

/// <summary>
///     Command to create a new <c>Something</c>/>.
/// </summary>
public sealed record CreateSomethingCommand(
    CreateSomethingRequest Request,
    Guid UserId)
    : ICommand<Guid>;