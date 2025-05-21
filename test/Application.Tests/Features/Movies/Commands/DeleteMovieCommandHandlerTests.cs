using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Movies.Commands.DeleteMovie;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;

namespace Application.Tests.Features.Movies.Commands;

/// <summary>
/// Unit tests for DeleteMovieCommandHandler.
/// </summary>
public class DeleteMovieCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMovieRepository> _movieRepoMock;
    private readonly DeleteMovieCommandHandler _handler;

    public DeleteMovieCommandHandlerTests()
    {
        // Arrange common mocks
        _uowMock = new Mock<IUnitOfWork>();
        _movieRepoMock = new Mock<IMovieRepository>();

        // Wire up unitOfWork.Movies and CommitAsync
        _uowMock
            .Setup(u => u.Movies)
            .Returns(_movieRepoMock.Object);
        _uowMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Instantiate handler
        _handler = new DeleteMovieCommandHandler(_uowMock.Object);
    }

    /// <summary>
    /// Test to ensure that when a movie exists, it is deleted and Unit is returned.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMovieExists_DeletesAndReturnsUnit()
    {
        // Arrange: existing domain movie
        var movieId = Guid.NewGuid();
        var domainMovie = new Movie { Id = movieId };
        _movieRepoMock
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainMovie);

        var command = new DeleteMovieCommand(movieId);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: SoftDelete and CommitAsync invoked
        _movieRepoMock.Verify(r => r.SoftDelete(It.Is<Movie>(m => m.Id == movieId)), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.Should().Be(Unit.Value);
    }

    /// <summary>
    /// Test to ensure that when a movie is not found, a NotFoundException is thrown.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMovieNotFound_ThrowsNotFoundException()
    {
        // Arrange: repository returns null
        var movieId = Guid.NewGuid();
        _movieRepoMock
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        var command = new DeleteMovieCommand(movieId);

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert: NotFoundException and no SoftDelete/Commit
        await act.Should()
                 .ThrowAsync<NotFoundException>()
                 .WithMessage($"Movie with Id {movieId} not found.");
        _movieRepoMock.Verify(r => r.SoftDelete(It.IsAny<Movie>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
