namespace TaskService.Application.DTOs;

/// <summary>
/// Safe outbound user representation — PasswordHash is intentionally excluded.
/// Embedded inside LoginResponseDto and used for /api/auth/me.
/// </summary>
public sealed record UserDto(
    int Id,
    string Username,
    string Email,
    bool IsActive,
    DateTime CreatedAt
);