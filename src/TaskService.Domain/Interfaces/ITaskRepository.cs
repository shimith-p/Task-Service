using TaskService.Domain.Models;

namespace TaskService.Domain.Interfaces
{
    public interface ITaskRepository
    {
        /// <summary>Persists a new task.</summary>
        Task<int> AddAsync(TasksModel model, CancellationToken cancellationToken = default);

        /// <summary>Returns the task with the given id, or null if not found.</summary>
        Task<TasksModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>Returns all tasks ordered by creation date descending.</summary>
        Task<IEnumerable<TasksModel>> GetAllAsync(CancellationToken cancellationToken = default); 

        /// <summary>Applies the updated domain model to the existing persisted task.</summary>
        Task UpdateAsync(TasksModel model, CancellationToken cancellationToken = default);

        /// <summary>Removes the task with the given id if it exists.</summary>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
