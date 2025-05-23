using MediatR;

namespace Application.Features.Movies.Commands.SyncMovies;

/// <summary>
/// Command to synchronize movies.
/// </summary>
public record SyncMoviesCommand() : IRequest<Unit>;
