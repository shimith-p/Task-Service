using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskService.Application.DTOs;

/// <summary>
/// Describes the content of an error
/// </summary>
public class ErrorResponseItemDto
{
    /// <summary>
    /// Error Code
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string ErrorCode { get; }

    /// <summary>
    /// Error Message
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; }

    /// <summary>
    /// Generates a new instance of <see cref="ErrorResponseItemDto"/>.
    /// </summary>
    public ErrorResponseItemDto(string errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
