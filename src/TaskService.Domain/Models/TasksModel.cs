using System;
using System.Collections.Generic;
using System.Text;
using TaskService.Domain.Enums;

namespace TaskService.Domain.Models
{
    public class TasksModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal OriginalEstimatedWork { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
