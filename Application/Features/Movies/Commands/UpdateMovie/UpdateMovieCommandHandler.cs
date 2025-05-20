using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Movies.Commands.UpdateMovie;

public class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand, MovieDetailDto>
{
    private readonly IUnitOfWork _uow;

    public UpdateMovieCommandHandler(IUnitOfWork uow)
        => _uow = uow;

    public async Task<MovieDetailDto> Handle(UpdateMovieCommand request, CancellationToken ct)
    {
        // 1. Retrieve existing entity or fail
        var movie = await _uow.Movies.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException($"Movie with Id {request.Id} not found.");

        // 2. Apply updates
        movie.Title = request.Data.Title;
        movie.Director = request.Data.Director;
        movie.ReleaseDate = (DateTime)request.Data.ReleaseDate;

        // 3. Persist the changes
        _uow.Movies.Update(movie);
        await _uow.CommitAsync(ct);

        // 4. Check if the movie was updated
        var updatedMovie = await _uow.Movies.GetByIdAsync(request.Id, ct);
        if (updatedMovie == null)
            throw new NotFoundException($"Movie with Id {request.Id} not found after update.");

        // 5. Return the updated movie
        return (MovieDetailDto)updatedMovie;
    }
}
