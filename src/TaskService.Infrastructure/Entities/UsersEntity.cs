namespace TaskService.Domain.Entities;

/// <summary>
/// EF Core persistence entity for a user account.
/// Lives exclusively in the Infrastructure layer's DbContext —
/// never exposed to the Application or API layers directly.
///
/// SECURITY: PasswordHash stores a BCrypt/PBKDF2 hash — never a plaintext password.
/// Hashing is the responsibility of the Application layer before calling Create().
/// </summary>
public sealed class UsersEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    private UsersEntity() { }  // Required by EF Core

    /// <summary>
    /// Creates a new UserEntity.
    /// <paramref name="passwordHash"/> must already be a hashed value —
    /// this method never accepts or stores a raw password.
    /// </summary>
    public static UsersEntity Create(
        string username,
        string email,
        string passwordHash)
    {
        return new UsersEntity
        {
            Username = username,
            Email = email,
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>Updates profile fields. Does not touch PasswordHash.</summary>
    public void UpdateProfile(string username, string email)
    {
        Username = username;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Replaces the password hash. Caller must supply a freshly hashed value.</summary>
    public void ChangePasswordHash(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate() { IsActive = false; UpdatedAt = DateTime.UtcNow; }
    public void Activate() { IsActive = true; UpdatedAt = DateTime.UtcNow; }
}