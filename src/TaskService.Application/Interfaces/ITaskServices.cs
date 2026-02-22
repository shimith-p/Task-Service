using TaskService.Application.DTOs;

namespace TaskService.Application.Interfaces;

/// <summary>
/// Application service contract consumed by the API layer.
/// Operates exclusively on DTOs — no domain models or EF entities leak into the controller.
/// </summary>
public interface ITaskServices
{
    /// <summary>
    /// Validates and creates a new task. Returns the created task.
    /// </summary>
    Task<TaskResponseDto> CreateAsync(CreateTaskDto? dto, CancellationToken cancellationToken = default);

    /// <summary>Returns a single task by id. Throws NotFoundException if absent.</summary>
    Task<TaskResponseDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Returns all tasks ordered by creation date descending.</summary>
    Task<IEnumerable<TaskResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Validates and fully replaces a task. Throws NotFoundException if absent.</summary>
    Task<TaskResponseDto> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken cancellationToken = default);

    /// <summary>Deletes a task by id. Throws NotFoundException if absent.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}