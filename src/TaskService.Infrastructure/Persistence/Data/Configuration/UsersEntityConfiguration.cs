using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Data.Configurations;

/// <summary>
/// Fluent API configuration for the Users table.
/// All column types, constraints, and indexes are defined here — not on the entity.
/// </summary>
public sealed class UsersEntityConfiguration : IEntityTypeConfiguration<UsersEntity>
{
    public void Configure(EntityTypeBuilder<UsersEntity> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();              // SQL Server IDENTITY(1,1)

        // Username — unique, case-insensitive collation recommended in production
        builder.Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(u => u.Username)
            .IsUnique()
            .HasDatabaseName("UX_Users_Username");

        // Email — unique
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(256);               // RFC 5321 max email length

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("UX_Users_Email");

        // PasswordHash — stores BCrypt/PBKDF2 hash, never plaintext
        // BCrypt output is 60 chars; use 256 to accommodate other algorithms
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(u => u.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");
    }
}