using TaskService.Application.DTOs;

namespace TaskService.Application.Interfaces;

/// <summary>
/// Contract for all authentication operations.
/// The API layer depends on this — never on AuthService directly.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates credentials and returns a signed JWT + refresh token.
    /// Throws <see cref="Exceptions.UnauthorizedException"/> on bad credentials or inactive account.
    /// </summary>
    Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the profile of the currently authenticated user.
    /// Throws <see cref="Exceptions.NotFoundException"/> if the user no longer exists.
    /// </summary>
    Task<UserDto> GetCurrentUserAsync(
        int userId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all active refresh tokens for the user — effectively ending all sessions.
    /// </summary>
    Task LogoutAsync(
        int userId,
        CancellationToken cancellationToken = default);
}