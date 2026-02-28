using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Interfaces;
using TaskService.Domain.Models;
using TaskService.Infrastructure.Data;
using TaskService.Infrastructure.Mappers;
using TaskService.Infrastructure.Persistence.Data;

namespace TaskService.Infrastructure.Repositories;

/// <summary>
/// Concrete implementation of IAuthRepository.
///
/// Owns two concerns:
///   1. Credential lookup — targeted queries for login (fetches user including PasswordHash).
///   2. Refresh token lifecycle — add, get, revoke per-token and all-tokens-for-user.
///
/// The Application layer (AuthService) only ever sees IAuthRepository —
/// never AppDbContext, UserEntity, or RefreshTokenEntity directly.
/// </summary>
public sealed class AuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public AuthRepository(AppDbContext context)
    {
        _context = context;
    }

    // ── Credential lookup ─────────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task<UsersModel?> FindByUsernameAsync(
        string username,
        CancellationToken cancellationToken = default)
    { 
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);

        return entity is null ? null : UserEntityMapper.ToModel(entity);
    }

    /// <inheritdoc/>
    public async Task<UsersModel?> FindByIdAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        return entity is null ? null : UserEntityMapper.ToModel(entity);
    }

    // ── Refresh token — Add ───────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task<RefreshTokenModel> AddRefreshTokenAsync(
        RefreshTokenModel model,
        CancellationToken cancellationToken = default)
    {
        var entity = RefreshTokenEntityMapper.ToEntity(model);

        await _context.RefreshTokens.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        // EF Core back-fills entity.Id after SaveChangesAsync — map back to capture it
        return RefreshTokenEntityMapper.ToModel(entity);
    }

    // ── Refresh token — Get ───────────────────────────────────────────────────

    /// <inheritdoc/>
    public async Task<RefreshTokenModel?> GetRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

        return entity is null ? null : RefreshTokenEntityMapper.ToModel(entity);
    }

    // ── Refresh token — Revoke single ─────────────────────────────────────────

    /// <inheritdoc/>
    public async Task RevokeRefreshTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        // Tracking query — we need EF to detect the Revoke() mutation
        var entity = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token, cancellationToken);

        if (entity is null || entity.IsRevoked)
            return;  // Idempotent — already revoked or doesn't exist

        entity.Revoke();
        await _context.SaveChangesAsync(cancellationToken);
    }

    // ── Refresh token — Revoke all for user ───────────────────────────────────

    /// <inheritdoc/>
    public async Task RevokeAllUserRefreshTokensAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        // Load only active (non-revoked) tokens — skip already-revoked ones
        var activeTokens = await _context.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ToListAsync(cancellationToken);

        if (activeTokens.Count == 0)
            return;

        foreach (var token in activeTokens)
            token.Revoke();

        await _context.SaveChangesAsync(cancellationToken);
    }
}