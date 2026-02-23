using Microsoft.AspNetCore.Mvc;
using TaskService.Application.DTOs;
using TaskService.Application.Interfaces;

namespace TaskService.Api.Controllers;

[ApiController]
[Route("v1/api/tasks")]
[Produces("application/json")]
public sealed class TasksController : ControllerBase
{
    private readonly ITasksService _taskServices;
    private readonly ILogger<TasksController> _logger;

    public TasksController(ITasksService taskServices, ILogger<TasksController> logger)
    {
        _taskServices = taskServices ?? throw new ArgumentNullException(nameof(taskServices)); ;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>Creates a new task.</summary>
    /// <param name="dto">The task payload to create.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    /// <response code="201">Task created.</response>
    /// <response code="400">Task request is bad.</response>
    /// <response code="409">A task with the same unique constraint already exists.</response>
    /// <response code="422">One or more validation errors in the request body.</response>
    /// <response code="500">An unexpected server error occurred.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)] 
    [ProducesResponseType(typeof(void), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(void), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(void), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTaskAsync(
        [FromBody] CreateTaskDto dto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating task. Title={Title}", dto?.Title); 

        var created = await _taskServices.CreateTaskAsync(dto, cancellationToken);

        _logger.LogInformation("Task created. Id={TaskId}", created.Id);

        return CreatedAtAction(nameof(GetTaskByIdAsync), new { id = created.Id }, created);
    }

    /// <summary>Gets a single task by its unique identifier.</summary>
    /// <param name="id">The task's id.</param>
    /// <response code="200">Task found and returned.</response>
    /// <response code="400">The id route value is not a valid integer.</response>
    /// <response code="404">No task with the given id exists.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponsePayloadDto), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTaskByIdAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for get task by id. Id={TaskId} ErrorCount={ErrorCount}", id, ModelState.ErrorCount);
            return ValidationProblem(ModelState);
        }

        _logger.LogDebug("Getting task by id. Id={TaskId}", id);

        var task = await _taskServices.GetTaskByIdAsync(id, cancellationToken);

        return Ok(task);
    }

    /// <summary>Lists all tasks ordered by creation date (newest first).</summary>
    /// <response code="200">Array of tasks. Returns an empty array when none exist.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTasksAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Getting all tasks");

        var tasks = await _taskServices.GetTasksAsync(cancellationToken);
        return Ok(tasks);
    }

    /// <summary>UpdateTaskAsync exist task.</summary>
    /// <param name="id">The task's id.</param> 
    /// <response code="200">Task updated successfully. Returns the updated task.</response>
    /// <response code="404">No task with the given id exists.</response>
    /// <response code="422">One or more validation errors in the request body.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TaskResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateTaskAsync(
        [FromRoute] int id,
        [FromBody] UpdateTaskDto dto,
        CancellationToken cancellationToken)
    { 
        _logger.LogInformation("Updating task. Id={TaskId}", id);

        var updated = await _taskServices.UpdateTaskAsync(id, dto, cancellationToken);
        _logger.LogInformation("Task updated. Id={TaskId}", id);
        return Ok(updated);
    }

    /// <summary>Deletes a task permanently.</summary>
    /// <param name="id">The task's GUID.</param>
    /// <response code="204">Task deleted. No content returned.</response>
    /// <response code="404">No task with the given id exists.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTaskAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for delete task. Id={TaskId} ErrorCount={ErrorCount}", id, ModelState.ErrorCount);
            return ValidationProblem(ModelState);
        }

        _logger.LogInformation("Deleting task. Id={TaskId}", id);

        await _taskServices.DeleteTaskAsync(id, cancellationToken);
        _logger.LogInformation("Task deleted. Id={TaskId}", id);
        return NoContent();
    }
}