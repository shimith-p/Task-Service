namespace TaskService.Application.Exceptions;

/// <summary>
/// Thrown when authentication fails — invalid credentials, inactive account, etc.
/// Caught by ExceptionMiddleware and translated to HTTP 401 Unauthorized.
/// </summary>
public sealed class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }

    // Factory for the most common case — keeps service code clean
    public static UnauthorizedException InvalidCredentials() =>
        new("Invalid username or password.");

    public static UnauthorizedException AccountInactive() =>
        new("This account has been deactivated. Please contact support.");
}