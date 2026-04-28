namespace Taskly.Shared.Pagination;

public sealed class PaginationResult<T>(IEnumerable<T> items, int pageIndex, int totalPages, int totalCount)
{
    public IEnumerable<T> Items { get; } = items;
    public int PageIndex { get; } = pageIndex;
    public int TotalPages { get; } = totalPages;
    public int TotalCount { get; } = totalCount;
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}