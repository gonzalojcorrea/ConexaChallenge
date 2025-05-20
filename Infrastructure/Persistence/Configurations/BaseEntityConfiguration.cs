using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Configurations;

public static class BaseEntityConfiguration
{
    /// <summary>
    /// Applies the base entity configuration to all entities inheriting from BaseEntity.
    /// </summary>
    /// <param name="modelBuilder"></param>
    public static void ApplyBaseEntityConfiguration(this ModelBuilder modelBuilder)
    {
        var baseEntityType = typeof(BaseEntity);

        foreach (var entityType in modelBuilder.Model
                 .GetEntityTypes()
                 .Where(t => baseEntityType.IsAssignableFrom(t.ClrType)))
        {
            var builder = modelBuilder.Entity(entityType.ClrType);

            // Configure the primary key
            builder.Property(nameof(BaseEntity.Id))
                   .ValueGeneratedOnAdd()
                   .HasColumnName("Id")
                   .IsRequired();

            // Configure the CreatedAt and DeletedAt properties
            builder.Property(nameof(BaseEntity.CreatedAt))
                   .ValueGeneratedOnAdd();
            builder.Property(nameof(BaseEntity.DeletedAt))
                   .IsRequired(false);

            // Configure the soft delete filter
            builder.HasQueryFilter(
                CreateIsDeletedRestriction(entityType.ClrType)
            );
        }
    }

    /// <summary>
    /// Lambda expression to filter out soft-deleted entities.
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    private static LambdaExpression CreateIsDeletedRestriction(Type entityType)
    {
        var param = Expression.Parameter(entityType, "e");
        var convertedParam = Expression.Convert(param, typeof(object));
        var propertyAccess = Expression.Call(
            typeof(EF),
            nameof(EF.Property),
            new[] { typeof(DateTime?) },
            convertedParam,
            Expression.Constant(nameof(BaseEntity.DeletedAt))
        );
        var body = Expression.Equal(
            propertyAccess,
            Expression.Constant(null, typeof(DateTime?))
        );
        return Expression.Lambda(body, param);
    }
}
