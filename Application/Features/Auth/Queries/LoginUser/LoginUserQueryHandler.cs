using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Queries.LoginUser;

/// <summary>
/// Query to log in a user and return a JWT token.
/// </summary>
public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string>
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IJwtService _jwtGen;

    public LoginUserQueryHandler(
        IUnitOfWork uow,
        IPasswordHasher<User> hasher,
        IJwtService jwtGen)
    {
        _uow = uow;
        _hasher = hasher;
        _jwtGen = jwtGen;
    }

    public async Task<string> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        // Validate the request
        var user = await _uow.Users.GetByUsernameAsync(request.Username)
            ?? throw new BadRequestException("Usuario o contraseña inválidos.");

        // Check if the user is active
        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        // Check if the password is correct
        if (result == PasswordVerificationResult.Failed)
            throw new BadRequestException("Usuario o contraseña inválidos.");

        // Generate a JWT token for the user
        return _jwtGen.GenerateToken(user);
    }
}
