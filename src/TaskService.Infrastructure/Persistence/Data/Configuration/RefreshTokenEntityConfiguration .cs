using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Data.Configurations;

/// <summary>
/// Fluent API configuration for the RefreshTokens table.
/// </summary>
public sealed class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();  // SQL Server IDENTITY(1,1)

        // Opaque random token string — SHA-256 hex is 64 chars; use 128 for flexibility
        builder.Property(r => r.Token)
            .IsRequired()
            .HasMaxLength(128);

        // Unique index — token strings must never collide
        builder.HasIndex(r => r.Token)
            .IsUnique()
            .HasDatabaseName("UX_RefreshTokens_Token");

        builder.Property(r => r.ExpiresAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(r => r.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(r => r.IsRevoked)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.RevokedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        // ── FK → Users ────────────────────────────────────────────────────────
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade)  // Delete user → delete their tokens too
            .HasConstraintName("FK_RefreshTokens_Users_UserId");

        // Index for common queries: "get all tokens for user X"
        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");

        // Index for expiry cleanup jobs
        builder.HasIndex(r => r.ExpiresAt)
            .HasDatabaseName("IX_RefreshTokens_ExpiresAt");
    }
}