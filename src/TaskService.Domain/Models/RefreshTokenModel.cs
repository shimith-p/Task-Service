namespace TaskService.Domain.Models;

/// <summary>
/// Immutable domain model for a refresh token.
/// Crosses layer boundaries above Infrastructure — Application layer uses this type.
/// </summary>
public sealed class RefreshTokenModel
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRevoked { get; init; }
    public DateTime? RevokedAt { get; init; }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}