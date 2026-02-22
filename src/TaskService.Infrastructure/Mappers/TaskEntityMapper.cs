
using TaskService.Domain.Enums;
using TaskService.Domain.Models;
using TaskService.Infrastructure.Entities;

namespace TaskService.Infrastructure.Mappers;

/// <summary>
/// Internal-only mapper between the EF Core entity (TaskItem) and the domain model (TaskModel).
/// Marked internal so nothing outside Infrastructure can reference it.
/// This is the only place in the codebase that knows about both types simultaneously.
/// </summary>
internal static class TaskEntityMapper
{
    /// <summary>Maps a persisted EF entity to an immutable domain model for upward use.</summary>
    internal static TasksModel ToModel(TasksEntity entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Description = entity.Description,
        Status = entity.Status.ToString(),
        OriginalEstimatedWork = entity.OriginalEstimatedWork,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    /// <summary>Creates a new EF entity from a domain model (used on Add).</summary>
    internal static TasksEntity ToEntity(TasksModel model) =>
        TasksEntity.Create(
            model.Title,
            model.Description,
            model.OriginalEstimatedWork,
            Enum.TryParse<TasksStatus>(model.Status, out var status) ? status : TasksStatus.Todo);

    /// <summary>
    /// Applies domain model values onto an existing tracked EF entity (used on Update).
    /// Avoids a delete + re-insert and lets EF detect and write only the changed columns.
    /// </summary>
    internal static void ApplyChanges(TasksEntity entity, TasksModel model) =>
        entity.Update(
            model.Title,
            model.Description,
            Enum.TryParse<TasksStatus>(model.Status, out var status) ? status : TasksStatus.Todo,
            model.OriginalEstimatedWork);
}