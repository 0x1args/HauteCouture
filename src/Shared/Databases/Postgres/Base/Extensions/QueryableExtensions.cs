using HauteCouture.Shared.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace HauteCouture.Shared.Databases.Postgres.Extensions;

/// <summary>
///     Extension methods for <see cref="IQueryable{TEntity}"/> providing pagination support.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    ///     Executes the query and returns a single page of results according to <paramref name="filter"/>.
    ///     Performs two database round-trips: one for the total count and one for the page items.
    /// </summary>
    /// <typeparam name="TEntity">The entity type being managed.</typeparam>
    /// <param name="source">The query to paginate.</param>
    /// <param name="filter">Paging parameters (page number and page size).</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    ///     <see cref="PagedList{TEntity}"/> containing the total record count, current page metadata, and page items.
    /// </returns>
    public static async Task<OffsetPagedList<TEntity>> ToPagedListAsync<TEntity>(
        this IQueryable<TEntity> source,
        PagedFilter filter,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(filter);

        var totalCount = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new OffsetPagedList<TEntity>(totalCount, filter.PageNumber, filter.PageSize, items);
    }

    /// <summary>
    ///     Executes the query and returns a single page of results using keyset (cursor-based) pagination.
    ///     Performs a single database round-trip and does not require a <c>COUNT</c> query.
    ///     Provides consistent performance regardless of how deep into the dataset the caller has navigated.
    /// </summary>
    /// <remarks>
    ///     Caller is responsible for applying the keyset filter (e.g. <c>WHERE Id &gt; @lastSeenId</c>)
    ///     and the <c>ORDER BY</c> clause to <paramref name="source"/> before calling this method.
    ///     The <c>ORDER BY</c> key must be unique; for non-unique sort columns add the primary key
    ///     as a tie-breaker both in the <c>WHERE</c> predicate and in <c>ORDER BY</c>.
    /// </remarks>
    /// <typeparam name="TEntity">The entity type being managed.</typeparam>
    /// <param name="source">
    ///     Pre-filtered and pre-ordered query to paginate.
    /// </param>
    /// <param name="pageSize">Maximum number of items to return per page.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>
    ///     <see cref="KeysetPage{TEntity}"/> containing the page items and a flag indicating
    ///     whether more records exist after the current page.
    /// </returns>
    public static async Task<CursorPagedList<TEntity>> ToKeysetPageAsync<TEntity>(
        this IQueryable<TEntity> source,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(pageSize);

        var items = await source
            .Take(pageSize + 1)
            .ToListAsync(cancellationToken);

        var hasNextPage = items.Count > pageSize;

        if (hasNextPage)
            items.RemoveAt(pageSize);

        return new CursorPagedList<TEntity>
        {
            Items = items,
            HasNextPage = hasNextPage
        };
    }
}