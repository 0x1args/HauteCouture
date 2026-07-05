using HauteCouture.Shared.CQS.Primitives.Commands;
using MediatR;

namespace HauteCouture.Shared.CQS.Abstractions.Handlers;

/// <summary>
///     Command handler without result.
/// </summary>
/// <typeparam name="TCommand">Type of command.</typeparam>
public interface ICommandHandler<in TCommand>
    : IRequestHandler<TCommand>
    where TCommand : ICommand;

/// <summary>
///     Command handler with result.
/// </summary>
/// <typeparam name="TCommand">Type of command.</typeparam>
/// <typeparam name="TResponse">Command result type.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;