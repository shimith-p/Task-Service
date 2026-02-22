using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Entities;

namespace TaskService.Infrastructure.Data.Configurations;

/// <summary>
/// Fluent API configuration for the <see cref="TaskEntity"/> EF entity.
/// Column types, constraints, and indexes are all defined here — not on the entity itself.
/// </summary>
public sealed class TaskEntityConfiguration : IEntityTypeConfiguration<TasksEntity>
{
    public void Configure(EntityTypeBuilder<TasksEntity> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .IsRequired()
            .ValueGeneratedOnAdd()  // SQL Server IDENTITY(1,1) — DB assigns the int on INSERT
            .UseIdentityColumn();   // Explicit: starts at 1, increments by 1

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .IsRequired(false)
            .HasMaxLength(2000);

        // ── StatusId FK ──────────────────────────────────────────────────────
        // StatusId is the persisted FK column pointing to TaskStatuses.Id.
        // The computed Status property (enum accessor) is explicitly ignored below.
        builder.Property(t => t.StatusId)
            .IsRequired()
            .HasDefaultValue(1);   // Default: Todo (Id = 1)

        builder.HasOne(t => t.TaskStatus)
            .WithMany()
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict)  // Prevent accidental status row deletion
            .HasConstraintName("FK_Tasks_TaskStatuses_StatusId");

        // Ignore the computed enum accessor — EF must not map it to a column
        builder.Ignore(t => t.Status);

        builder.Property(t => t.OriginalEstimatedWork)
            .IsRequired()
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0m);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.HasIndex(t => t.StatusId)
            .HasDatabaseName("IX_Tasks_StatusId");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_Tasks_CreatedAt");
    }
}