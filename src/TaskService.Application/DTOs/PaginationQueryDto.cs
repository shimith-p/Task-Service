namespace TaskService.Application.DTOs;

public sealed class PaginationQueryDto
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}