namespace HauteCouture.Shared.Common.Pagination;

/// <summary>
///     Filter for pagination.
/// </summary>
public record PagedFilter
{
    /// <summary>
    ///     Maximum allowed page size.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    ///     Minimum allowed page size.
    /// </summary>
    public const int MinPageSize = 0;

    /// <summary>
    ///     Page number (starting from 1).
    /// </summary>
    public int PageNumber
    {
        get;
        set => field = value > MinPageSize ? value : 1;
    } = 1;

    /// <summary>
    ///     Page size (number of items per page).
    /// </summary>
    public int PageSize
    {
        get;
        set => field = value is > MinPageSize and <= MaxPageSize ? value : 10;
    } = 10;

    /// <summary>
    ///     Creates a new instance of <see cref="PagedFilter"/>.
    /// </summary>
    /// <param name="pageNumber">Page number (1 by default).</param>
    /// <param name="pageSize">Page size (default 10, maximum 100).</param>
    public PagedFilter(int pageNumber = 1, int pageSize = 10)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}