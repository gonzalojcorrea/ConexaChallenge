using Application.Features.Auth.Commands.RegisterUser;
using Application.Features.Auth.Queries.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
    /// <param name="cmd"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand cmd)
    {
        var token = await _mediator.Send(cmd);
        return Ok(token);
    } 

    /// <summary>
    /// Logs in an existing user and returns a JWT token.
    /// </summary>
    /// <param name="qry"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserQuery qry)
    {
        var token = await _mediator.Send(qry);
        return Ok(token);
    }
}
