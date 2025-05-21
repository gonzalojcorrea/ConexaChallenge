using Application.Common.Dtos;
using MediatR;

namespace Application.Features.Movies.Queries.GetMovies;

/// <summary>
/// Query para obtener todas las películas.
/// </summary>
public record GetMoviesQuery() : IRequest<IEnumerable<MovieDto>>;
