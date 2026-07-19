namespace HauteCouture.Shared.CQS.Primitives.Queries;

/// <summary>
///     Pagination parameters for queries that support paging.
/// </summary>
public interface IPagedQuery
{
    /// <summary>
    ///     Page number for the query.
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    ///     Page size for the query (number of items per page).
    /// </summary>
    int PageSize { get; }
}