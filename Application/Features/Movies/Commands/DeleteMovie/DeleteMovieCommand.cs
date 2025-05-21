using MediatR;

namespace Application.Features.Movies.Commands.DeleteMovie;

public record DeleteMovieCommand(Guid Id) : IRequest<Unit>;
