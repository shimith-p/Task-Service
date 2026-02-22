namespace TaskService.API.Models;

/// <summary>
/// Consistent error envelope returned for every non-2xx response.
/// Consumers always receive the same shape regardless of error type.
/// </summary>
/// <param name="StatusCode">The HTTP status code (mirrors the response status).</param>
/// <param name="Message">Human-readable summary of the error.</param>
/// <param name="Errors">
/// Field-level validation errors keyed by property name.
/// Only present on 422 Unprocessable Entity responses.
/// </param>
public sealed record ApiErrorResponse(
    int StatusCode,
    string Message,
    IReadOnlyDictionary<string, string[]>? Errors = null
);