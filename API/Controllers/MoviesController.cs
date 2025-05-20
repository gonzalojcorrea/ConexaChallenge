using Application.Common.Dtos;
using Application.Common.Models;
using Application.Features.Movies.Commands.CreateMovie;
using Application.Features.Movies.Commands.UpdateMovie;
using Application.Features.Movies.Queries.GetMovieById;
using Application.Features.Movies.Queries.GetMovies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for managing movies.
/// </summary>
[ApiController]
[Route("api/movies")]
public class MoviesController : ControllerBase
{
    private readonly ISender _mediator;

    public MoviesController(ISender mediator)
        => _mediator = mediator;

    /// <summary>
    /// Retrieves all movies.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(SuccessResponse<IEnumerable<MovieDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var movies = await _mediator.Send(new GetMoviesQuery(), ct);
        return Ok(movies);
    }

    /// <summary>
    /// Retrieves a movie by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:guid}")]
    [Authorize(Roles = "Admin, User")]
    [ProducesResponseType(typeof(SuccessResponse<MovieDetailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var movie = await _mediator.Send(new GetMovieByIdQuery(id), ct);
        return Ok(movie);
    }

    /// <summary>
    /// Creates a new movie.
    /// </summary>
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateMovieCommand cmd, CancellationToken ct)
    {
        var id = await _mediator.Send(cmd, ct);
        return Ok("The movie was created.");
    }

    /// <summary>
    /// Updates an existing movie.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cmd"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateMovieDto cmd, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateMovieCommand(id, cmd), ct);
        return Ok(result);
    }
}
