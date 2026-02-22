namespace TaskService.Application.Exceptions;

/// <summary>
/// Exception to validate conflict
/// </summary>
public class ConflictException : Exception
{
    /// <summary>
    /// Generates a new instance of <see cref="ConflictException"/>.
    /// </summary>
    public ConflictException(string message)
        : base(message)
    {
    }
}
