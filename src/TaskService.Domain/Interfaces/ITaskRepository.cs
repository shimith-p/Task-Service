using TaskService.Domain.Models;

namespace TaskService.Domain.Interfaces
{
    public interface ITaskRepository
    {
        /// <summary>Persists a new task.</summary>
        Task<int> AddTaskAsync(TasksModel model, CancellationToken cancellationToken = default);

        /// <summary>Returns the task with the given id, or null if not found.</summary>
        Task<TasksModel?> GetTaskByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Returns all tasks ordered by creation date descending.</summary>
        Task<IEnumerable<TasksModel>> GetTasksAsync(CancellationToken cancellationToken = default); 

        /// <summary>Applies the updated domain model to the existing persisted task.</summary>
        Task UpdateTaskAsync(TasksModel model, CancellationToken cancellationToken = default);

        /// <summary>Removes the task with the given id if it exists.</summary>
        Task DeleteTaskAsync(int id, CancellationToken cancellationToken = default);
    }
}
