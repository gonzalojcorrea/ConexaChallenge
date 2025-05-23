using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Auth.Queries.LoginUser;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Application.Tests.Features.Auth.Queries;

/// <summary>
/// Unit tests for the LoginUserQueryHandler.
/// </summary>
public class LoginUserQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher<User>> _hasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly LoginUserQueryHandler _handler;

    public LoginUserQueryHandlerTests()
    {
        // Arrange common mocks
        _uowMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _hasherMock = new Mock<IPasswordHasher<User>>();
        _jwtServiceMock = new Mock<IJwtService>();

        // Wire up uof.Users
        _uowMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        // Instantiate handler under test
        _handler = new LoginUserQueryHandler(
            _uowMock.Object,
            _hasherMock.Object,
            _jwtServiceMock.Object);
    }

    /// <summary>
    /// Test to ensure that the handler throws a BadRequestException when the user is not found.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenUserNotFound_ThrowsBadRequestException()
    {
        // Arrange: repository returns null
        var username = "nonexistent";
        _userRepoMock
            .Setup(r => r.GetByEmailAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var query = new LoginUserQuery(username, "anyPwd");

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should()
                 .ThrowAsync<BadRequestException>()
                 .WithMessage("Usuario o contraseña inválidos.");
    }

    /// <summary>
    /// Test to ensure that the handler throws a BadRequestException when the password is incorrect.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenPasswordIncorrect_ThrowsBadRequestException()
    {
        // Arrange: repository returns a user
        var user = new User { Email = "user1", PasswordHash = "hash" };
        _userRepoMock
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Hasher fails verification
        _hasherMock
            .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "wrongPwd"))
            .Returns(PasswordVerificationResult.Failed);

        var query = new LoginUserQuery(user.Email, "wrongPwd");

        // Act
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should()
                 .ThrowAsync<BadRequestException>()
                 .WithMessage("Usuario o contraseña inválidos.");
    }

    /// <summary>
    /// Test to ensure that the handler returns a JWT token when the credentials are valid.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenCredentialsValid_ReturnsJwtToken()
    {
        // Arrange: repository returns a user
        var user = new User { Email = "user2", PasswordHash = "hash2" };
        _userRepoMock
            .Setup(r => r.GetByEmailAsync(user.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Hasher succeeds verification
        _hasherMock
            .Setup(h => h.VerifyHashedPassword(user, user.PasswordHash, "correctPwd"))
            .Returns(PasswordVerificationResult.Success);

        // JWT service returns a token
        var expectedToken = "jwt-token";
        _jwtServiceMock
            .Setup(j => j.GenerateToken(user))
            .Returns(expectedToken);

        var query = new LoginUserQuery(user.Email, "correctPwd");

        // Act
        var token = await _handler.Handle(query, CancellationToken.None);

        // Assert
        token.Should().Be(expectedToken);
    }
}
