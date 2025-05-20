using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Movies.Dtos;
using MediatR;

namespace Application.Features.Movies.Queries.GetMovies;

/// <summary>
/// Query to get all movies.
/// </summary>
public class GetMoviesQueryHandler : IRequestHandler<GetMoviesQuery, IEnumerable<MovieDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMoviesQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<MovieDto>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
    {
        // Fetch all movies from the database
        var movies = await _unitOfWork.Movies.GetAllAsync(cancellationToken);

        // Check if the movies were fetched successfully
        if (movies == null || !movies.Any())
            throw new NotFoundException("No movies found.");

        // Map the movies to DTOs
        return movies.Select(m => new MovieDto
        {
            Id = m.Id,
            Title = m.Title
        });
    }
}
