using HauteCouture.Example.Contracts.Requests;
using HauteCouture.Shared.CQS.Primitives.Commands;

namespace HauteCouture.Example.Applications.Handlers.Commands.UpdateSomethingDescription;

/// <summary>
///     Command to update the description of an existing <c>Something</c>.
/// </summary>
public sealed record UpdateSomethingDescriptionCommand(
    Guid SomethingId,
    UpdateSomethingDescriptionRequest Request)
    : ICommand;