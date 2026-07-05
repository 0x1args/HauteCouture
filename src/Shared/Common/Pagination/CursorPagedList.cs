namespace HauteCouture.Shared.Common.Pagination;

/// <summary>
///     Represents a single page of results produced by keyset (cursor-based) pagination.
///     Unlike offset pagination, keyset pagination provides consistent performance regardless
///     of how deep into the dataset the caller has navigated.
/// </summary>
/// <typeparam name="TData">The element type of the page.</typeparam>
public sealed class CursorPagedList<TData>
{
    /// <summary>Items on the current page.</summary>
    public required IReadOnlyList<TData> Items { get; init; }

    /// <summary>
    ///     Indicates whether more records exist after the current page.
    ///     When <see langword="false"/>, the caller has reached the end of the dataset.
    /// </summary>
    public required bool HasNextPage { get; init; }
}