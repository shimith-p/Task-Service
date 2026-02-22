using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskService.Domain.Entities;

namespace TaskService.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the <c>TaskStatuses</c> lookup table and seeds the three fixed rows.
/// The seed Id values (1, 2, 3) must remain in sync with the
/// <see cref="TaskService.Domain.Enums.TaskStatus"/> enum values.
/// </summary>
public sealed class TaskStatusEntityConfiguration : IEntityTypeConfiguration<TaskStatusEntity>
{
    public void Configure(EntityTypeBuilder<TaskStatusEntity> builder)
    {
        builder.ToTable("TaskStatuses");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .IsRequired()
            .ValueGeneratedNever();   // Ids are fixed / seeded — never auto-generated

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(s => s.Name)
            .IsUnique()
            .HasDatabaseName("UX_TaskStatuses_Name");

        // ── Seed data ─────────────────────────────────────────────────────────
        // These three rows are the source of truth for valid statuses.
        // Id values mirror TaskStatus enum: Todo=1, InProgress=2, Done=3
        builder.HasData(
            TaskStatusEntity.Create(1, "Todo"),
            TaskStatusEntity.Create(2, "InProgress"),
            TaskStatusEntity.Create(3, "Done")
        );
    }
}