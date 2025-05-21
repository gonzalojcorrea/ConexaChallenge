using MediatR;

namespace Application.Features.Auth.Queries.LoginUser;

/// <summary>
/// Query to log in a user and return a JWT token.
/// </summary>
/// <param name="Username"></param>
/// <param name="Password"></param>
public record LoginUserQuery(string Username, string Password)
    : IRequest<string>;
