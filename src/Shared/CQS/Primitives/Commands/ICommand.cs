using MediatR;

namespace HauteCouture.Shared.CQS.Primitives.Commands;

/// <summary>
///     Marker interface used to identify commands for pipeline 
///     behaviors that must apply only to commands.
/// </summary>
public interface IBaseCommand;

/// <summary>
///     Indicates the command without returning a result.
/// </summary>
public interface ICommand : IRequest, IBaseCommand;

/// <summary>
///     Indicates the command with the expected result.
/// </summary>
/// <typeparam name="TResponse">Type of command result.</typeparam>
public interface ICommand<TResponse> : IRequest<TResponse>, IBaseCommand;