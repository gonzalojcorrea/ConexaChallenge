using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

/// <summary>
/// Database context for the application.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// Configures the model for the context.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyBaseEntityConfiguration();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <returns></returns>
    public override int SaveChanges()
        => ApplySoftDeleteRules(() => base.SaveChanges());

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        => ApplySoftDeleteRules(() => base.SaveChangesAsync(ct));

    /// <summary>
    /// Applies soft delete rules to the entities in the context.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="baseSave"></param>
    /// <returns></returns>
    private T ApplySoftDeleteRules<T>(Func<T> baseSave)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAt = utcNow;
                    break;
            }
        }

        return baseSave();
    }
}
