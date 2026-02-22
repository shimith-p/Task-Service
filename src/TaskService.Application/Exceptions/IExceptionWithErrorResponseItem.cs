using TaskService.Application.DTOs;

namespace TaskService.Application.Exceptions;

/// <summary>
/// Interface for exceptions containing error content
/// </summary>
public interface IExceptionWithErrorResponseItem
{
    /// <summary>
    /// List of validation errors
    /// </summary>
    List<ErrorResponseItemDto> Errors { get; }
}
