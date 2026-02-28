using FluentValidation;
using TaskService.Application.DTOs;
using TaskService.Application.Interfaces;
using TaskService.Application.Mapping;
using TaskService.Application.Services;
using TaskService.Application.Validation;
using TaskService.Domain.Interfaces;
using TaskService.Infrastructure.Persistence.Repositories;
using TaskService.Infrastructure.Repositories;

namespace TaskService.Api.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<TaskMappingProfile>());
            services.AddScoped<ITasksService, TasksService>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IValidator<CreateTaskDto>, CreateTaskDtoValidator>();
            services.AddScoped<IValidator<UpdateTaskDto>, UpdateTaskDtoValidator>();
            services.AddScoped<IValidator<LoginRequestDto>, LoginRequestValidator>();
            return services;
        
        }
    }
}
