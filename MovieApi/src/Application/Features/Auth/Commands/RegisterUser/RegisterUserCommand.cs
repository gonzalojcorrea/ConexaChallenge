using MediatR;

namespace Application.Features.Auth.Commands.RegisterUser;

/// <summary>
/// Command to register a new user.
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="Role"></param>
public record RegisterUserCommand(string Email, string Password, string Role) 
    : IRequest<string>;
