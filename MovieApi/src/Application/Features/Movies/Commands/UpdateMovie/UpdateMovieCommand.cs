using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Movies.Commands.UpdateMovie;

/// <summary>
/// Command to update an existing movie.
/// </summary>
/// <param name="Id"></param>
/// <param name="Data"></param>
public record UpdateMovieCommand(Guid Id, UpdateMovieDto Data) : IRequest<MovieDetailDto>;

/// <summary>
/// Data Transfer Object (DTO) for updating a movie.
/// </summary>
public class UpdateMovieDto
{
    public string Title { get; set; }
    public string Director { get; set; }
    public string Producer { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string OpeningCrawl { get; set; }
    public List<string> Characters { get; set; }
}
