namespace TaskService.Application.DTOs;

/// <summary>
/// Response DTO returned from POST /api/auth/login.
/// Contains everything the client needs to authenticate subsequent requests.
/// </summary>
public sealed record LoginResponseDto(
    string AccessToken,    // JWT Bearer token — send in Authorization header
    string TokenType,      // Always "Bearer"
    int ExpiresIn,      // Seconds until the access token expires
    DateTime ExpiresAt,      // Absolute UTC expiry time
    UserDto User            // Basic user info — saves a separate /me call
);