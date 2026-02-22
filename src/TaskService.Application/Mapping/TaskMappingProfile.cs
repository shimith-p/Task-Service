using AutoMapper;
using TaskService.Application.DTOs;
using TaskService.Domain.Models;

namespace TaskService.Application.Mapping;

public sealed class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        // ── TasksModel → TaskResponseDto ─────────────────────────────────────
        // All properties are name-matched; no custom config needed.
        CreateMap<TasksModel, TaskResponseDto>();

        // ── CreateTaskDto → TaskModel ───────────────────────────────────────
        // Id and timestamps are generated here, not by the DB or the repository.
        CreateMap<CreateTaskDto, TasksModel>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        // ── UpdateTaskDto → TaskModel ───────────────────────────────────────
        // Id and CreatedAt are supplied by the service via `with { }` after mapping.
        CreateMap<UpdateTaskDto, TasksModel>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.CreatedAt, opt => opt.Ignore())
            .ForMember(d => d.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
