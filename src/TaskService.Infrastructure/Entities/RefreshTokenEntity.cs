namespace TaskService.Domain.Entities;

/// <summary>
/// EF Core persistence entity for a refresh token.
/// Each row represents one issued refresh token tied to a user.
/// Multiple rows per user are valid (different devices/sessions).
/// </summary>
public sealed class RefreshTokenEntity
{
    public int Id { get; private set; }  // DB-generated IDENTITY
    public int UserId { get; private set; }  // FK → Users.Id
    public string Token { get; private set; } = string.Empty;  // Opaque random token
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    // Navigation property — EF relationship only
    public UsersEntity? User { get; private set; }

    private RefreshTokenEntity() { }  // Required by EF Core

    public static RefreshTokenEntity Create(int userId, string token, DateTime expiresAt) =>
        new()
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }

    public bool IsActive => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}