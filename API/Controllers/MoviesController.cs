using Application.Common.Dtos;
using Application.Common.Models;
using Application.Features.Movies.Commands.CreateMovie;
using Application.Features.Movies.Commands.DeleteMovie;
using Application.Features.Movies.Commands.SyncMovies;
using Application.Features.Movies.Commands.UpdateMovie;
using Application.Features.Movies.Queries.GetMovieById;
using Application.Features.Movies.Queries.GetMovies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of movies wrapped in a SuccessResponse.</returns>
    [HttpGet]
    [Authorize(Roles = "Master, Padawan")]
    [ProducesResponseType(typeof(SuccessResponse<IEnumerable<MovieDto>>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all movies",
        Description = "Retrieves all movies. Requires Admin or User role."
    )]
    [SwaggerResponse(200, "Movies retrieved successfully", typeof(SuccessResponse<IEnumerable<MovieDto>>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ErrorResponse))]
    [SwaggerResponse(403, "Forbidden", typeof(ErrorResponse))]
    [SwaggerResponse(404, "No movies found", typeof(ErrorResponse))]
    [SwaggerResponse(500, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var movies = await _mediator.Send(new GetMoviesQuery(), ct);
        return Ok(movies);
    }

    /// <summary>
    /// Retrieves a movie by its ID.
    /// </summary>
    /// <param name="id">The GUID of the movie.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A single movie wrapped in a SuccessResponse.</returns>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Padawan")]
    [ProducesResponseType(typeof(SuccessResponse<MovieDetailDto>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get movie by ID",
        Description = "Retrieves a movie by its GUID. Requires Admin or User role."
    )]
    [SwaggerResponse(200, "Movie retrieved successfully", typeof(SuccessResponse<MovieDetailDto>))]
    [SwaggerResponse(401, "Unauthorized", typeof(ErrorResponse))]
    [SwaggerResponse(403, "Forbidden", typeof(ErrorResponse))]
    [SwaggerResponse(404, "Movie not found", typeof(ErrorResponse))]
    [SwaggerResponse(500, "Internal server error", typeof(ErrorResponse))]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var movie = await _mediator.Send(new GetMovieByIdQuery(id), ct);
        return Ok(movie);
    }

    /// <summary>
    /// Creates a new movie.
    /// </summary>
    /// <param name="cmd">Movie creation data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>201 Created with Location header pointing to the new movie.</returns>
    [HttpPost]
    [Authorize(Roles = "Master")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Create a new movie",
        Description = "Creates a new movie record. Requires Admin role."
    )]
    [SwaggerResponse(201, "Movie created successfully", typeof(SuccessResponse<string>))]
    public async Task<IActionResult> Create(
        [SwaggerRequestBody("Movie creation payload", Required = true)]
        [FromBody] CreateMovieCommand cmd,
        CancellationToken ct)
    {
        var id = await _mediator.Send(cmd, ct);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Updates an existing movie.
    /// </summary>
    /// <param name="id">The GUID of the movie to update.</param>
    /// <param name="cmd">Updated movie data.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>200 OK with updated movie.</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Master")]
    [ProducesResponseType(typeof(SuccessResponse<MovieDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Update an existing movie",
        Description = "Updates the details of an existing movie. Requires Admin role."
    )]
    [SwaggerResponse(200, "Movie updated successfully", typeof(SuccessResponse<MovieDetailDto>))]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [SwaggerRequestBody("Movie update payload", Required = true)]
        [FromBody] UpdateMovieDto cmd,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateMovieCommand(id, cmd), ct);
        return Ok(result);
    }

    /// <summary>
    /// Synchronize the Star Wars movies from the external API.
    /// </summary>
    /// <returns>A 200 OK status indicating successful synchronization.</returns>
    [HttpPost("sync")]
    [Authorize(Roles = "Master")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status502BadGateway)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status504GatewayTimeout)]
    [SwaggerOperation(
        Summary = "Synchronize movies",
        Description = "Fetches and updates the movie list from the Star Wars API. Requires Admin role."
    )]
    [SwaggerResponse(200, "Movies synchronized successfully", typeof(object))]
    public async Task<IActionResult> Sync()
    {
        await _mediator.Send(new SyncMoviesCommand());
        return Ok(new { message = "Movies synchronized successfully." });
    }

    /// <summary>
    /// Deletes an existing movie by its ID.
    /// </summary>
    /// <param name="id">The GUID of the movie to delete.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Master")]
    [ProducesResponseType(typeof(SuccessResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [SwaggerOperation(
        Summary = "Delete a movie",
        Description = "Deletes the movie with the given GUID. Requires Admin role."
    )]
    [SwaggerResponse(200, "Movie deleted successfully")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _mediator.Send(new DeleteMovieCommand(id));
        return Ok("Movie deleted successfully");
    }
}
