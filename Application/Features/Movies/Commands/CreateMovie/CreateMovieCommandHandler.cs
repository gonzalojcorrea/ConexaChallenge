using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Application.Features.Movies.Commands.CreateMovie;

/// <summary>
/// Command to create a new movie.
/// </summary>
public class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, Guid>
{
    private readonly IUnitOfWork _uof;

    public CreateMovieCommandHandler(IUnitOfWork uof)
    {
        _uof = uof;
    }

    public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        if (await _uof.Movies.GetByTitleAsync(request.Title, cancellationToken) is not null)
            throw new BadRequestException("El título de la película ya está en uso.");

        // Create a new movie
        var movie = new Movie
        {
            Title = request.Title,
            Director = request.Director,
            ReleaseDate = request.ReleaseDate
        };

        // Add the movie to the database
        await _uof.Movies.AddAsync(movie, cancellationToken);
        await _uof.CommitAsync(cancellationToken);

        // Return the ID of the created movie
        return movie.Id;
    }
}
