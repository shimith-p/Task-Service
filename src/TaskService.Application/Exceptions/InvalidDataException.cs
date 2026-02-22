namespace TaskService.Application.Exceptions;

/// <summary>
/// Thrown by the Application layer when a requested resource does not exist.
/// Caught by <c>ExceptionMiddleware</c> and translated to HTTP 404 Not Found.
/// </summary>
public sealed class InvalidDataException : Exception
{
    public string ResourceName { get; }
    public object ResourceKey { get; }

    public InvalidDataException(string resourceName, object key)
        : base($"{resourceName} with id '{key}' was not found.")
    {
        ResourceName = resourceName;
        ResourceKey = key;
    }
}