using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

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
        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.Property(m => m.Director)
            .HasMaxLength(100);
        builder.Property(m => m.Producer)
            .HasMaxLength(100);
        builder.Property(m => m.OpeningCrawl)
            .HasMaxLength(2000);

        builder.Property(m => m.ReleaseDate)
            .HasColumnType("timestamp with time zone")
            .HasConversion(
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

        builder.Property(m => m.Characters)
            .HasColumnType("jsonb")
            .HasMaxLength(2000)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)!);
    }
}
