using TaskService.Domain.Entities;
using TaskService.Domain.Models;

namespace TaskService.Infrastructure.Mappers;

/// <summary>
/// Internal-only mapper between RefreshTokenEntity (EF Core) and RefreshTokenModel (domain).
/// </summary>
internal static class RefreshTokenEntityMapper
{
    internal static RefreshTokenModel ToModel(RefreshTokenEntity entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        Token = entity.Token,
        ExpiresAt = entity.ExpiresAt,
        CreatedAt = entity.CreatedAt,
        IsRevoked = entity.IsRevoked,
        RevokedAt = entity.RevokedAt
    };

    internal static RefreshTokenEntity ToEntity(RefreshTokenModel model) =>
        RefreshTokenEntity.Create(model.UserId, model.Token, model.ExpiresAt);
}