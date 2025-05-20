using MediatR;

namespace Application.Features.Movies.Commands.CreateMovie;

/// <summary>
/// Command to create a new movie.
/// </summary>
/// <param name="Title"></param>
/// <param name="Director"></param>
/// <param name="ReleaseDate"></param>
public record CreateMovieCommand(string Title, string Director, DateTime ReleaseDate)
    : IRequest<Guid>;
