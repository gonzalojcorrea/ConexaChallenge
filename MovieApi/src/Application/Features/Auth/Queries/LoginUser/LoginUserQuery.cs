using MediatR;

namespace Application.Features.Auth.Queries.LoginUser;

/// <summary>
/// Query to log in a user and return a JWT token.
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record LoginUserQuery(string Email, string Password)
    : IRequest<string>;
