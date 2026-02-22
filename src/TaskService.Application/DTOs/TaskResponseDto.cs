using TaskService.Domain.Enums;

namespace TaskService.Application.DTOs;

public sealed class TaskResponseDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal OriginalEstimatedWork { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}