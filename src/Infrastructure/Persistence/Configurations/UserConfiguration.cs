using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for the User entity.
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // Configure the table name and primary key
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        // Configure the properties
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(30);
        builder.Property(u => u.PasswordHash)
            .IsRequired();

        // Configure the relationships
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
