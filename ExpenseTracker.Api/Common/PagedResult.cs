namespace ExpenseTracker.Api.Common;

public class PagedResult<T>(
    IReadOnlyList<T> items,
    int totalCount,
    int pageIndex,
    int pageSize
)
{
    public IReadOnlyList<T> Items { get; init; } = items;
    public int PageIndex { get; init; } = pageIndex;
    public int PageSize { get; init; } = pageSize;
    public int TotalCount { get; init; } = totalCount;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageIndex > 1;
    public bool HasNextPage => PageIndex < TotalPages;
}