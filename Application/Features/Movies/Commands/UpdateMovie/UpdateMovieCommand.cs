using Application.Features.Movies.Dtos;
using MediatR;

namespace Application.Features.Movies.Commands.UpdateMovie;

/// <summary>
/// Command to update an existing movie.
/// </summary>
public record UpdateMovieCommand(Guid Id, UpdateMovieDto Data) : IRequest<MovieDetailDto>;