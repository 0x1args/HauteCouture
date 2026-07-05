using MediatR;

namespace HauteCouture.Shared.CQS.Primitives.Queries;

/// <summary>
///     Indicates the query with the expected result.
/// </summary>
/// <typeparam name="TResponse">Type of query result.</typeparam>
public interface IQuery<TResponse> : IRequest<TResponse>;