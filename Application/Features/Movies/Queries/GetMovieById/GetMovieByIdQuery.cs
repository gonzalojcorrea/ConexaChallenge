using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Movies.Queries.GetMovieById;

/// <summary>
/// Query to get a movie by its ID.
/// </summary>
/// <param name="Id"></param>
public record GetMovieByIdQuery(Guid Id) : IRequest<MovieDetailDto>;
