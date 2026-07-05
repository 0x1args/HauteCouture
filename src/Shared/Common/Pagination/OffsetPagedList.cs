namespace HauteCouture.Shared.Common.Pagination;

/// <summary>
///     Presentation of a paginated list of data with metadata.
/// </summary>
/// <typeparam name="TData">The element type of the page.</typeparam>
public sealed class OffsetPagedList<TData>
{
    /// <summary>
    ///     Total number of items.
    /// </summary>
    public int TotalCount { get; }

    /// <summary>
    ///     Current page number (starting from 1).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    ///     Page size (number of items per page).
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    ///     Total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    ///     Indicates whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    ///     Indicates whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    ///     List of data for current page.
    /// </summary>
    public List<TData> Items { get; }

    /// <summary>
    ///     Creates a new instance of <see cref="OffsetPagedList{TData}"/>.
    /// </summary>
    /// <param name="totalCount">Total number of items.</param>
    /// <param name="pageNumber">Current page number (starting from 1).</param>
    /// <param name="pageSize">Total number of pages.</param>
    /// <param name="items">List of data for current page.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OffsetPagedList(int totalCount, int pageNumber, int pageSize, List<TData> items)
    {
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        Items = items ?? throw new ArgumentNullException(nameof(items));
    }
}