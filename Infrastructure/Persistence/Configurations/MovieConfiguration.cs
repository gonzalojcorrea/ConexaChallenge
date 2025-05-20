using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuration for the Movie entity.
/// </summary>
public class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        // Configure the table name and primary key
        builder.ToTable("Movies");
        builder.HasKey(m => m.Id);

        // Configure the properties
        builder.Property(m => m.Id)
            .ValueGeneratedOnAdd();
        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(m => m.Director)
            .HasMaxLength(100);
    }
}
