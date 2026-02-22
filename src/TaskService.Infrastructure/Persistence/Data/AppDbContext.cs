using Microsoft.EntityFrameworkCore;
using TaskService.Domain.Entities;
using TaskService.Infrastructure.Entities;

namespace TaskService.Infrastructure.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TasksEntity> Tasks => Set<TasksEntity>();
        public DbSet<TaskStatusEntity> TaskStatuses => Set<TaskStatusEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Auto-discover all IEntityTypeConfiguration<T> in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically update UpdatedAt on every save (belt-and-suspenders alongside domain logic)
            foreach (var entry in ChangeTracker.Entries<TasksEntity>()
                         .Where(e => e.State == EntityState.Modified))
            {
                entry.Property(nameof(TasksEntity.UpdatedAt)).CurrentValue = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
