using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Auth.Commands.RegisterUser;

/// <summary>
/// Command to register a new user.
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, string>
{
    private readonly IUnitOfWork _uof;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtService _jwtTokenGenerator;

    public RegisterUserCommandHandler(
        IUnitOfWork uof,
        IPasswordHasher<User> passwordHasher,
        IJwtService jwtTokenGenerator)
    {
        _uof = uof;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<string> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        if (await _uof.Users.GetByEmailAsync(request.Email) is not null)
            throw new BadRequestException("El email ingresado ya está en uso.");

        // Check if the role exists
        var role = await _uof.Roles.GetByNameAsync(request.Role)
            ?? throw new NotFoundException($"El rol '{request.Role}' no existe.");

        // Create a new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            RoleId = role.Id,
        };

        // Hash the password
        var hashed = _passwordHasher.HashPassword(user, request.Password);
        user.PasswordHash = hashed;

        // Add the user to the database
        await _uof.Users.AddAsync(user);
        await _uof.CommitAsync(cancellationToken);

        // Generate a JWT token for the user
        return _jwtTokenGenerator.GenerateToken(user);
    }
}
