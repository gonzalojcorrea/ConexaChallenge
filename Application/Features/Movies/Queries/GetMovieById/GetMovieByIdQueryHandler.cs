using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Movies.Queries.GetMovieById;

public class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, MovieDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMovieByIdQueryHandler(IUnitOfWork unitOfWork)
        => _unitOfWork = unitOfWork;

    public async Task<MovieDetailDto> Handle(GetMovieByIdQuery request, CancellationToken ct)
    {
        // 1) Fetch the entity or throw if not found
        var movie = await _unitOfWork.Movies.GetByIdAsync(request.Id, ct)
                    ?? throw new NotFoundException($"Movie with Id {request.Id} not found.");

        return new MovieDetailDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            ReleaseDate = movie.ReleaseDate
        };
    }
}
