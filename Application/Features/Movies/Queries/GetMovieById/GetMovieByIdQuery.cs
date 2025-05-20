using Application.Features.Movies.Dtos;
using MediatR;

namespace Application.Features.Movies.Queries.GetMovieById;

/// <summary>
/// Query to get details of a single movie.
/// </summary>
public record GetMovieByIdQuery(Guid Id) : IRequest<MovieDetailDto>;
