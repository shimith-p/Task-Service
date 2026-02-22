using FluentValidation.Results;

namespace TaskService.Application.Exceptions;

/// <summary>
/// Thrown by the Application layer when incoming data fails FluentValidation rules.
/// Caught by <c>ExceptionMiddleware</c> and translated to HTTP 422 Unprocessable Entity.
///
/// <para>
/// The <see cref="Errors"/> dictionary groups all failure messages by property name so that
/// API consumers receive a structured, field-level error response:
/// </para>
///
/// <code>
/// {
///   "statusCode": 422,
///   "message": "One or more validation errors occurred.",
///   "errors": {
///     "Title":                 ["Title is required."],
///     "OriginalEstimatedWork": ["OriginalEstimatedWork must be 0 or greater."]
///   }
/// }
/// </code>
/// </summary>
public sealed class ValidationException : Exception
{
    /// <summary>
    /// Field-level validation failures.
    /// Key   = property name (e.g. "Title", "OriginalEstimatedWork").
    /// Value = one or more human-readable error messages for that field.
    /// </summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    // ── Constructors ─────────────────────────────────────────────────────────

    /// <summary>
    /// Initialises the exception from a pre-built errors dictionary.
    /// Used when you want full control over the error structure.
    /// </summary>
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }

    /// <summary>
    /// Initialises the exception directly from a FluentValidation
    /// <see cref="ValidationResult"/>. Groups failures by property name.
    /// </summary>
    public ValidationException(ValidationResult validationResult)
        : base("One or more validation errors occurred.")
    {
        Errors = BuildErrors(validationResult.Errors);
    }

    /// <summary>
    /// Initialises the exception from a raw list of FluentValidation failures.
    /// Useful when aggregating failures from multiple validators.
    /// </summary>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = BuildErrors(failures);
    }

    // ── Factory helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Creates a <see cref="ValidationException"/> for a single field failure.
    /// Convenience method for simple manual validation in application services.
    /// </summary>
    /// <example>
    /// throw ValidationException.For("Title", "Title is required.");
    /// </example>
    public static ValidationException For(string propertyName, string errorMessage) =>
        new(new Dictionary<string, string[]>
        {
            { propertyName, new[] { errorMessage } }
        });

    /// <summary>
    /// Creates a <see cref="ValidationException"/> for multiple failures on a single field.
    /// </summary>
    public static ValidationException For(string propertyName, params string[] errorMessages) =>
        new(new Dictionary<string, string[]>
        {
            { propertyName, errorMessages }
        });

    // ── Private helpers ──────────────────────────────────────────────────────

    private static IReadOnlyDictionary<string, string[]> BuildErrors(
        IEnumerable<ValidationFailure> failures) =>
        failures
            .GroupBy(f => f.PropertyName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).Distinct().ToArray(),
                StringComparer.OrdinalIgnoreCase);
}