using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Movies.Commands.CreateMovie;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Movies.Commands;

/// <summary>
/// Unit tests for CreateMovieCommandHandler.
/// </summary>
public class CreateMovieCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uofMock;
    private readonly Mock<IMovieRepository> _movieRepoMock;
    private readonly CreateMovieCommandHandler _handler;

    public CreateMovieCommandHandlerTests()
    {
        // Arrange common mocks
        _uofMock = new Mock<IUnitOfWork>();
        _movieRepoMock = new Mock<IMovieRepository>();

        // Wire up unitOfWork.Movies and CommitAsync
        _uofMock.Setup(u => u.Movies).Returns(_movieRepoMock.Object);
        _uofMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(1);

        // Instantiate the handler under test
        _handler = new CreateMovieCommandHandler(_uofMock.Object);
    }

    /// <summary>
    /// Test to ensure that when a movie is created with a unique title,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenTitleIsUnique_ShouldAddMovieAndReturnId()
    {
        // Arrange: command with unique title
        var newId = Guid.NewGuid();
        var command = new CreateMovieCommand(
            Title: "Unique Title",
            Director: "Dir",
            Producer: "Prod",
            ReleaseDate: new DateTime(2025, 5, 21),
            OpeningCrawl: "Some crawl",
            Characters: new List<string> { "Char1", "Char2" }
        );

        // No existing movie with the same title
        _movieRepoMock
            .Setup(r => r.GetByTitleAsync(command.Title, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        // Capture the added movie and assign an Id
        _movieRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .Callback<Movie, CancellationToken>((m, ct) => m.Id = newId)
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(newId);
        _movieRepoMock.Verify(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()), Times.Once);
        _uofMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    /// Test to ensure that when a movie is created with a duplicate title,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenTitleExists_ThrowsBadRequestException()
    {
        // Arrange: repository returns a movie with the same title
        var existing = new Movie { Id = Guid.NewGuid(), Title = "Duplicate" };
        _movieRepoMock
            .Setup(r => r.GetByTitleAsync("Duplicate", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var command = new CreateMovieCommand(
            Title: "Duplicate",
            Director: "Dir",
            Producer: "Prod",
            ReleaseDate: new DateTime(2025, 5, 21),
            OpeningCrawl: "Some crawl",
            Characters: new List<string>()
        );

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should()
            .ThrowAsync<BadRequestException>()
            .WithMessage("El título de la película ya está en uso.");
        _movieRepoMock.Verify(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()), Times.Never);
        _uofMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
