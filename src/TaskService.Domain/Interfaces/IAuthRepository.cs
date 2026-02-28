using TaskService.Domain.Models;

namespace TaskService.Domain.Interfaces;

/// <summary>
/// Auth-specific repository contract.
/// Owns two concerns that IUserRepository deliberately does not:
///
///   1. Credential lookup — fetches user+hash in a single targeted query for login.
///      IUserRepository.GetByUsernameAsync loads the full UserModel which is fine
///      for profile reads, but for auth we want a focused query with no extra projections.
///
///   2. Refresh token lifecycle — store, retrieve, validate, revoke.
///      Refresh tokens are an auth concern only — nothing else in the app touches them.
///
/// AuthService depends on IAuthRepository. IUserRepository is no longer injected
/// into AuthService directly.
/// </summary>
public interface IAuthRepository
{
    // ── Credential lookup ─────────────────────────────────────────────────────

    /// <summary>
    /// Finds a user by username and returns the full UserModel including PasswordHash.
    /// Returns null if no user with that username exists.
    /// Used exclusively for login — do not use for general user reads.
    /// </summary>
    Task<UsersModel?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a user by their DB id and returns the full UserModel.
    /// Used for GET /api/auth/me after extracting userId from JWT claim.
    /// </summary>
    Task<UsersModel?> FindByIdAsync(
        int userId,
        CancellationToken cancellationToken = default);

    // ── Refresh token ─────────────────────────────────────────────────────────

    /// <summary>Persists a new refresh token and returns it with its DB-assigned Id.</summary>
    Task<RefreshTokenModel> AddRefreshTokenAsync(
        RefreshTokenModel model,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a refresh token by its opaque token string.
    /// Returns null if not found.
    /// </summary>
    Task<RefreshTokenModel?> GetRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a refresh token as revoked.
    /// Idempotent — safe to call even if already revoked.
    /// </summary>
    Task RevokeRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes ALL active refresh tokens for a user.
    /// Call this on logout or password change.
    /// </summary>
    Task RevokeAllUserRefreshTokensAsync(
        int userId,
        CancellationToken cancellationToken = default);
}