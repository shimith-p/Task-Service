using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskService.Application.DTOs;

/// <summary>
/// Payload for error response
/// </summary>
public sealed class ErrorResponsePayloadDto
{
    /// <summary>
    /// List of errors
    /// </summary>
    [JsonPropertyName("errors")]
    public List<ErrorResponseItemDto> Errors { get; set; }

    /// <summary>
    /// Generates a new instance of <see cref="ErrorResponsePayloadDto"/>.
    /// </summary>
    public ErrorResponsePayloadDto(List<ErrorResponseItemDto> errors)
    {
        Errors = errors;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
