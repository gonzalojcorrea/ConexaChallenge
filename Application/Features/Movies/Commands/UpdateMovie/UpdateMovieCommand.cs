using Application.Common.Dtos;
using Domain.Entities;
using MediatR;
using System.Runtime.CompilerServices;

namespace Application.Features.Movies.Commands.UpdateMovie;

/// <summary>
/// Command to update an existing movie.
/// </summary>
public record UpdateMovieCommand(Guid Id, UpdateMovieDto Data) : IRequest<MovieDetailDto>;

public class UpdateMovieDto
{
    public string Title { get; set; }
    public string Director { get; set; }
    public string Producer { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string OpeningCrawl { get; set; }
    public List<string> Characters { get; set; }
}
