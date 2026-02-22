using System;
using System.Collections.Generic;
using System.Text;
using TaskService.Domain.Entities;
using TaskService.Domain.Enums;

namespace TaskService.Infrastructure.Entities
{
    public class TasksEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        /// <summary>
        /// FK to <see cref="TaskStatusEntity"/>.
        /// Stored as an int that mirrors the <see cref="TaskStatus"/> enum value.
        /// </summary>
        public int StatusId { get; set; }

        /// <summary>
        /// Navigation property loaded by EF Core when needed.
        /// Not used in domain logic — exposed for EF relationship wiring only.
        /// </summary>
        public TaskStatusEntity? TaskStatus { get; set; }

        /// <summary>
        /// Typed enum accessor derived from <see cref="StatusId"/>.
        /// Domain and application logic always use this; never use StatusId directly.
        /// </summary>
        public TasksStatus Status => (TasksStatus)StatusId;

        public decimal OriginalEstimatedWork { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Parameterless constructor required by EF Core
        private TasksEntity() { }

        /// <summary>Creates a new TaskItem with a generated Id and UTC timestamps.</summary>
        public static TasksEntity Create(
            string title,
            string? description,
            decimal originalEstimatedWork,
            TasksStatus status = TasksStatus.Todo)
        {
            return new TasksEntity
            {
                Title = title,
                Description = description,
                StatusId = (int)status,   
                OriginalEstimatedWork = originalEstimatedWork,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        /// <summary>Applies field updates and refreshes UpdatedAt.</summary>
        public void Update(
            string title,
            string? description,
            TasksStatus status,
            decimal originalEstimatedWork)
        {
            Title = title;
            Description = description;
            StatusId = (int)status;
            OriginalEstimatedWork = originalEstimatedWork;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
