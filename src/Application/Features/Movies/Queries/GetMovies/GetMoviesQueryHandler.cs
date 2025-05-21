using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
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
        // 1. Fetch all movies from the database
        var movies = await _unitOfWork.Movies.GetAllAsync(cancellationToken);

        // 2. Check if any movies were found
        if (movies == null || !movies.Any())
            throw new NotFoundException("No movies found.");

        // 3. Map the movies to DTOs
        return movies.Select(m => new MovieDto
        {
            Id = m.Id,
            Title = m.Title
        });
    }
}
