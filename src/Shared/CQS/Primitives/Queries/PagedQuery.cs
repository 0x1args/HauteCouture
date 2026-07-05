namespace HauteCouture.Shared.CQS.Primitives.Queries;

/// <summary>
///     Pagination parameters for queries that support paging.
/// </summary>
/// <typeparam name="TResponse">Response type.</typeparam>
/// <param name="PageNumber">Page number.</param>
/// <param name="PageSize">Page size.</param>
public record PagedQuery<TResponse>(
    int PageNumber,
    int PageSize) : IQuery<TResponse>, IPagedQuery;