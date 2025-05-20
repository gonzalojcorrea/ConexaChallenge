using MediatR;

namespace Application.Features.Movies.Commands.SyncMovies;

public record SyncMoviesCommand() : IRequest<Unit>;
