using MediatR;

namespace Application.Features.Movies.Commands.DeleteMovie;

/// <summary>
/// Command to delete a movie.
/// </summary>
/// <param name="Id"></param>
public record DeleteMovieCommand(Guid Id) : IRequest<Unit>;
