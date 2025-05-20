using Domain.Entities;
using System.Data;

namespace Application.Common.Dtos;

/// <summary>
/// Data Transfer Object for movie details.
/// </summary>
public class MovieDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Director { get; set; }
    public DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Converts a Movie entity to a MovieDetailDto.
    /// </summary>
    /// <param name="m"></param>
    public static explicit operator MovieDetailDto(Movie m)
    {
        return new MovieDetailDto
        {
            Id = m.Id,
            Title = m.Title,
            Director = m.Director,
            ReleaseDate = m.ReleaseDate
        };
    }
}
