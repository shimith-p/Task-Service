using TaskService.Application.DTOs;

namespace TaskService.Application.Interfaces;

/// <summary>
/// Application service contract consumed by the API layer.
/// Operates exclusively on DTOs — no domain models or EF entities leak into the controller.
/// </summary>
public interface ITasksService
{
    /// <summary>
    /// Creates a new task. Returns the created task.
    /// </summary>
    Task<TaskResponseDto> CreateTaskAsync(CreateTaskDto? dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a single task by id. 
    /// </summary>
    Task<TaskResponseDto> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all tasks ordered by creation date descending.
    /// </summary>
    Task<PagedResultDto<TaskResponseDto>> GetTasksAsync(PaginationQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a task. Throws NotFoundException if absent.
    /// </summary>
    Task<TaskResponseDto> UpdateTaskAsync(int id, UpdateTaskDto dto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a task by id. Throws NotFoundException if absent.
    /// </summary>
    Task DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
}