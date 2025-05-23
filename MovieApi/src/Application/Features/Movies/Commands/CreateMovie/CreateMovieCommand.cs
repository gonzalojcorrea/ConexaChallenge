using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Movies.Commands.CreateMovie;

/// <summary>
/// Command to create a new movie.
/// </summary>
/// <param name="Title"></param>
/// <param name="Director"></param>
/// <param name="Producer"></param>
/// <param name="ReleaseDate"></param>
/// <param name="OpeningCrawl"></param>
/// <param name="Characters"></param>
public record CreateMovieCommand(
    string Title,
    string Director,
    string Producer,
    DateTime ReleaseDate,
    string OpeningCrawl,
    List<string> Characters)
    : IRequest<Guid>;
