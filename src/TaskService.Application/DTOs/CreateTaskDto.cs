using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TaskService.Domain.Enums;

namespace TaskService.Application.DTOs;

public sealed class CreateTaskDto
{
    /// <summary>
    /// Title for task.
    /// </summary> 
    [JsonPropertyName("title")] 
    [RegularExpression(DtoConstants.ContainsNoHtmlTagValidationRegex, ErrorMessage = "Title contains invalid characters.")]
    public required string Title { get; set; }

    /// <summary>
    /// Description for task.
    /// </summary>
    [JsonPropertyName("description")] 
    [RegularExpression(DtoConstants.ContainsNoHtmlTagValidationRegex)]
    public string? Description { get; set; }

    /// <summary>
    /// Original estimated work for task (must be greater than 0).
    /// </summary>
    [JsonPropertyName("originalEstimatedWork")] 
    public required decimal OriginalEstimatedWork { get; set; }

    /// <summary>
    /// Status for task.
    /// </summary>
    [JsonPropertyName("status")] 
    public required TasksStatus Status { get; set; } = TasksStatus.Todo;
}