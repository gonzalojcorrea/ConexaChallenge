using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Movies.Commands.DeleteMovie;

public class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand, Unit>
{
    private readonly IUnitOfWork _uow;

    public DeleteMovieCommandHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<Unit> Handle(DeleteMovieCommand request, CancellationToken ct)
    {
        // 1. Retrieve existing entity or fail
        var movie = await _uow.Movies.GetByIdAsync(request.Id, ct)
                    ?? throw new NotFoundException($"Movie with Id {request.Id} not found.");

        // 2. Soft delete the movie
        _uow.Movies.SoftDelete(movie);
        await _uow.CommitAsync(ct);

        return Unit.Value;
    }
}
