namespace Moeen.Application.DTOs.Mosques;

/// <summary>
/// Represents a paginated result set.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public class PagedResult<T>
{
    /// <summary>Gets the items on the current page.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>Gets the total number of items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>Gets the current page number (1-based).</summary>
    public int PageNumber { get; init; }

    /// <summary>Gets the page size.</summary>
    public int PageSize { get; init; }

    /// <summary>Gets the total number of pages.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
