namespace TaskService.Application.DTOs;

public sealed class PagedResultDto<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required int PageNumber { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}