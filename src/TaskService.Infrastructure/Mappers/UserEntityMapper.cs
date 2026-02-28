using System;
using System.Collections.Generic;
using System.Text;
using TaskService.Domain.Entities;
using TaskService.Domain.Models;
using TaskService.Infrastructure.Entities;

namespace TaskService.Infrastructure.Mappers
{
    internal static class UserEntityMapper
    {
        internal static UsersModel ToModel(UsersEntity entity) => new()
        {
            Id = entity.Id,
            Username = entity.Username,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}
