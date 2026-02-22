using FluentValidation;
using TaskService.Application.DTOs;
using TaskService.Application.Interfaces;
using TaskService.Application.Mapping;
using TaskService.Application.Services;
using TaskService.Application.Validation;
using TaskService.Domain.Interfaces;
using TaskService.Infrastructure.Persistence.Repositories;

namespace TaskService.Api.Extensions
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<TaskMappingProfile>());
            services.AddScoped<ITaskServices, TaskServices>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IValidator<CreateTaskDto>, CreateTaskDtoValidator>();
            services.AddScoped<IValidator<UpdateTaskDto>, UpdateTaskDtoValidator>();
            return services;
        
        }
    }
}
