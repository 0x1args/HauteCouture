using HauteCouture.Shared.CQS.Primitives.Queries;
using MediatR;

namespace HauteCouture.Shared.CQS.Abstractions.Handlers;

/// <summary>
///     Query handler.
/// </summary>
/// <typeparam name="TQuery">Type of query.</typeparam>
/// <typeparam name="TResponse">Query result type.</typeparam>
public interface IQueryHandler<in TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;