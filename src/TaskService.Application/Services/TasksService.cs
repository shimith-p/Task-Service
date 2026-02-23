using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Logging;
using TaskService.Application.DTOs;
using TaskService.Application.Interfaces;
using TaskService.Domain.Interfaces;
using TaskService.Domain.Models;
using TaskService.Application.Exceptions;
using ValidationException = TaskService.Application.Exceptions.ValidationException;

namespace TaskService.Application.Services;

public sealed class TasksService : ITasksService
{
    private readonly ITaskRepository _repository;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateTaskDto> _createValidator;
    private readonly IValidator<UpdateTaskDto> _updateValidator;
    private readonly ILogger<TasksService> _logger;

    public TasksService(
        ITaskRepository repository,
        IMapper mapper,
        IValidator<CreateTaskDto> createValidator,
        IValidator<UpdateTaskDto> updateValidator,
        ILogger<TasksService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    } 

    /// <summary>
    /// Creates a new task using the specified DTO and returns a response containing the details of the created task.
    /// </summary>
    /// <remarks>The method validates the input data before creating the task. If validation fails, an
    /// exception is thrown. The returned response includes the generated task ID and other relevant details.</remarks>
    /// <param name="dto">The data transfer object that contains the information required to create the task. Cannot be null.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a TaskResponseDto with the details
    /// of the created task, including its generated identifier.</returns>
    public async Task<TaskResponseDto> CreateTaskAsync(
        CreateTaskDto? dto,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        _logger.LogDebug("Creating task (application). Title={Title}", dto.Title);

        //Validate input DTO. 
        await ValidateAsync(_createValidator, dto, cancellationToken);

        // Map DTO to domain model.
        var model = _mapper.Map<TasksModel>(dto);

        // Save to repository.
        var createdId = await _repository.AddTaskAsync(model, cancellationToken);

        // Update model with generated ID for logging and response mapping.
        model.Id = createdId;
        _logger.LogInformation("Task created. Id={TaskId} Title={Title}", model.Id, model.Title);

        // Map domain model to response DTO and return.
        return _mapper.Map<TaskResponseDto>(model);
    }

    /// <summary>
    /// Asynchronously retrieves a task by its ID.
    /// </summary>
    /// <param name="id">The ID of the task to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="TaskResponseDto"/> representing the task.</returns>
    public async Task<TaskResponseDto> GetTaskByIdAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting task by id (application). Id={TaskId}", id);

        // Get domain model by ID.
        var model = await GetOrThrowAsync(id, cancellationToken);

        // Map domain model to response DTO and return.
        return _mapper.Map<TaskResponseDto>(model);
    }

    /// <summary>
    /// Asynchronously retrieves all tasks as data transfer objects.
    /// </summary>
    /// <remarks>This method fetches all task entities from the underlying data store and maps them to
    /// response DTOs. Use the cancellation token to cancel the operation if needed.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of <see
    /// cref="TaskResponseDto"/> objects representing all tasks.</returns>
    public async Task<IEnumerable<TaskResponseDto>> GetTasksAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all tasks (application)");

        // Get all domain models from repository.
        var models = await _repository.GetTasksAsync(cancellationToken);

        if (models is ICollection<TasksModel> c)
        {
            _logger.LogDebug("Retrieved tasks (application). Count={Count}", c.Count);
        }

        // Map domain models to response DTOs and return.
        return _mapper.Map<IEnumerable<TaskResponseDto>>(models);
    } 

    /// <summary>
    /// Updates the task with the specified ID using the provided data transfer object (DTO).
    /// </summary>
    /// <param name="id">The ID of the task to update.</param>
    /// <param name="dto">The data transfer object containing the updated task information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task as a response DTO.</returns>
    /// <exception cref="NotFoundException">Thrown if a task with the specified ID does not exist.</exception>
    public async Task<TaskResponseDto> UpdateTaskAsync(
        int id,
        UpdateTaskDto dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Updating task (application). Id={TaskId}", id);

        // Validate input DTO.
        ArgumentNullException.ThrowIfNull(dto);

        // Validate input DTO.
        await ValidateAsync(_updateValidator, dto, cancellationToken);

        // Confirm existence first so we return 404, not silently no-op
        var existing = await GetOrThrowAsync(id, cancellationToken);

        // Map updated fields from DTO onto the existing model (preserves Id/CreatedAt).
        var updated = _mapper.Map(dto, existing);

        // Update in repository.
        await _repository.UpdateTaskAsync(updated, cancellationToken);

        _logger.LogInformation("Task updated. Id={TaskId}", updated.Id);

        // Map domain model to response DTO and return.
        return _mapper.Map<TaskResponseDto>(updated);
    }

    /// <summary>
    /// Deletes the task with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the task to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="NotFoundException">Thrown if a task with the specified ID does not exist.</exception>
    public async Task DeleteTaskAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Deleting task (application). Id={TaskId}", id);

        // Confirm existence first so we return 404, not silently no-op
        await GetOrThrowAsync(id, cancellationToken);

        // Delete from repository.
        await _repository.DeleteTaskAsync(id, cancellationToken);

        _logger.LogInformation("Task deleted. Id={TaskId}", id);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private async Task<TasksModel> GetOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        var model = await _repository.GetTaskByIdAsync(id, cancellationToken);

        if (model is null)
        {
            _logger.LogWarning("Task not found. Id={TaskId}", id);
            throw new NotFoundException("Task", id);
        }

        return model;
    } 

    private async Task ValidateAsync<T>(
        IValidator<T> validator,
        T instance,
        CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);

        if (!result.IsValid)
        {
            _logger.LogWarning("Validation failed (application). Type={Type} Errors={ErrorCount}", typeof(T).Name, result.Errors.Count);
            throw new ValidationException(result);  
        }
    }
}