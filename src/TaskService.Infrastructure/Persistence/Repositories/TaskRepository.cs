using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TaskService.Domain.Enums;
using TaskService.Domain.Interfaces;
using TaskService.Domain.Models;
using TaskService.Infrastructure.Mappers;
using TaskService.Infrastructure.Persistence.Data;

namespace TaskService.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TaskRepository> _logger;

    public TaskRepository(AppDbContext dbContext, ILogger<TaskRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<int> AddTaskAsync(
        TasksModel model,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Adding task (infrastructure). Title={Title}", model.Title);

        // Map the model to an entity.
        var entity = TaskEntityMapper.ToEntity(model);

        // Validate the title for uniqueness before adding.
        var title = entity.Title?.Trim();
        if (!string.IsNullOrWhiteSpace(title))
        {
            await ThrowIfTitleExistsAsync(title, cancellationToken);
        }

        // Add the entity to the context and save changes.
        await _dbContext.Tasks.AddAsync(entity, cancellationToken);

        // After saving, the entity's Id will be populated with the generated value.
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task persisted (infrastructure). Id={TaskId}", entity.Id);
        return entity.Id;
    }

    public async Task<TasksModel?> GetTaskByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting task by id (infrastructure). Id={TaskId}", id);

        // Load task + status name in one query (return status name instead of integer).
        var result = await (
            from t in _dbContext.Tasks.AsNoTracking()
            join s in _dbContext.TaskStatuses.AsNoTracking()
                on t.StatusId equals s.Id
            where t.Id == id
            select new { Task = t, StatusName = s.Name }
        ).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
            return null;

        var model = TaskEntityMapper.ToModel(result.Task);
        model.Status = result.StatusName;
        return model;
    }

    public async Task<IEnumerable<TasksModel>> GetTasksAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all tasks (infrastructure)");

        // Load tasks + status names (return status name instead of integer).
        var results = await (
            from t in _dbContext.Tasks.AsNoTracking()
            join s in _dbContext.TaskStatuses.AsNoTracking()
                on t.StatusId equals s.Id
            orderby t.CreatedAt descending
            select new { Task = t, StatusName = s.Name }
        ).ToListAsync(cancellationToken);

        return results.Select(r =>
        {
            var model = TaskEntityMapper.ToModel(r.Task);
            model.Status = r.StatusName;
            return model;
        });
    }

    public async Task UpdateTaskAsync(
        TasksModel model,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating task (infrastructure). Id={TaskId}", model.Id);

        // Get the existing entity.
        var entity = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == model.Id, cancellationToken);

        if (entity is null)
        {
            _logger.LogDebug("Task not found for update (infrastructure). Id={TaskId}", model.Id);
            return;
        }

        // Validate the title for uniqueness before updating.
        var title = model.Title?.Trim();
        if (!string.IsNullOrWhiteSpace(title))
        {
            await ThrowIfTitleExistsAsync(title, model.Id, cancellationToken);
        }

        // Map the changes from the model to the entity.
        TaskEntityMapper.ApplyChanges(entity, model);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task updated in store (infrastructure). Id={TaskId}", model.Id);
    }

    public async Task DeleteTaskAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting task (infrastructure). Id={TaskId}", id);

        // Get the entity to delete.
        var entity = await _dbContext.Tasks
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (entity is null)
        {
            _logger.LogDebug("Task not found for delete (infrastructure). Id={TaskId}", id);
            return;
        }

        // Remove the entity from the context and save changes.
        _dbContext.Tasks.Remove(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Task deleted from store (infrastructure). Id={TaskId}", id);
    }

    private async Task ThrowIfTitleExistsAsync( string? title, CancellationToken cancellationToken = default)
    {  
        var exists = await _dbContext.Tasks
            .AsNoTracking()
            .AnyAsync(t => t.Title == title, cancellationToken);

        if (exists)
        {
            _logger.LogWarning("Task title conflict (infrastructure). Title={Title}", title);
            throw new InvalidOperationException("Conflict: a task with the same title already exists.");
        } 
    }

    private async Task ThrowIfTitleExistsAsync( string? title,int id, CancellationToken cancellationToken = default)
    {
        var exists = await _dbContext.Tasks
        .AsNoTracking()
        .AnyAsync(t => (t.Title == title && t.Id != id), cancellationToken); 
        if (exists)
        {
            _logger.LogWarning("Task title conflict on update (infrastructure). Id={TaskId} Title={Title}", id, title);
            throw new InvalidOperationException("Conflict: a task with the same title already exists.");
        } 
    }
}
