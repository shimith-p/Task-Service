using System;
using System.Collections.Generic;
using System.Text;
using TaskService.Application.DTOs;
using TaskService.Domain.Models;

namespace TaskService.Application.Helpers
{
    public static class ModelMappingHelper
    {
         public static TasksModel ToCreateTaskModel(this CreateTaskDto dto)
         {
            return new TasksModel
            {
                Title = dto.Title,
                Description = dto.Description,
                OriginalEstimatedWork = dto.OriginalEstimatedWork,
                Status = dto.Status.ToString(),
            };
         }

         public static TaskResponseDto ToTaskResponseDto(this TasksModel model)
         {
            return new TaskResponseDto
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                OriginalEstimatedWork = model.OriginalEstimatedWork,
                Status = model.Status,
                CreatedAt = model.CreatedAt,
                UpdatedAt = model.UpdatedAt
            };
         }
    }
}