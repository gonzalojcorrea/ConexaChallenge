using Application.Common.Models;
using Application.Features.Auth.Commands.RegisterUser;
using Application.Features.Auth.Queries.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace API.Controllers;

/// <summary>
/// Controller for user authentication.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Registers a new user and returns a JWT token.
    /// </summary>
    /// <param name="cmd">Registration data: email, password, and role.</param>
    /// <returns>JWT token wrapped in a SuccessResponse.</returns>
    [HttpPost("sign-up")]
    [SwaggerOperation(
        Summary = "Register a new user",
        Description = "Creates a new user account with the specified role and returns an access JWT token."
    )]
    [SwaggerResponse(200, "User registered successfully", typeof(SuccessResponse<string>))]
    [SwaggerResponse(400, "Validation errors or bad request", typeof(ErrorResponse))]
    [SwaggerResponse(404, "Role not found", typeof(ErrorResponse))]
    [SwaggerResponse(500, "Internal server error", typeof(ErrorResponse))]
    public async Task<ActionResult<string>> Register(
        [SwaggerRequestBody("User registration payload", Required = true)]
        [FromBody] RegisterUserCommand cmd)
    {
        var token = await _mediator.Send(cmd);
        return Ok(token);
    }

    /// <summary>
    /// Logs in an existing user and returns a JWT token.
    /// </summary>
    /// <param name="qry">Login data: username and password.</param>
    /// <returns>JWT token wrapped in a SuccessResponse.</returns>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Login user",
        Description = "Authenticates a user and returns an access JWT token."
    )]
    [SwaggerResponse(200, "User authenticated successfully", typeof(SuccessResponse<string>))]
    [SwaggerResponse(400, "Invalid credentials", typeof(ErrorResponse))]
    [SwaggerResponse(500, "Internal server error", typeof(ErrorResponse))]
    public async Task<ActionResult<string>> Login(
        [SwaggerRequestBody("User login payload", Required = true)]
        [FromBody] LoginUserQuery qry)
    {
        var token = await _mediator.Send(qry);

        return Ok(token);
    }
}
