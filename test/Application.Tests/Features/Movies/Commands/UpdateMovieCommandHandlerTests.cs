using Application.Common.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Movies.Commands.UpdateMovie;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Movies.Commands;

/// <summary>
/// Unit tests for UpdateMovieCommandHandler.
/// </summary>
public class UpdateMovieCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMovieRepository> _movieRepoMock;
    private readonly UpdateMovieCommandHandler _handler;

    public UpdateMovieCommandHandlerTests()
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
        _handler = new UpdateMovieCommandHandler(_uowMock.Object);
    }

    /// <summary>
    /// Test to ensure that when a movie exists, it is updated and the DTO is returned.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMovieExists_UpdatesAndReturnsDto()
    {
        // Arrange: existing domain movie
        var movieId = Guid.NewGuid();
        var existing = new Movie
        {
            Id = movieId,
            Title = "Old Title",
            Director = "Old Dir",
            Producer = "Old Prod",
            ReleaseDate = new DateTime(2022, 1, 1),
            OpeningCrawl = "Old Crawl",
            Characters = new List<string> { "A" }
        };
        // Setup initial fetch
        _movieRepoMock
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Prepare update data
        var updateDto = new UpdateMovieDto
        {
            Title = "New Title",
            Director = "New Dir",
            Producer = "New Prod",
            ReleaseDate = new DateTime(2025, 5, 21),
            OpeningCrawl = "New Crawl",
            Characters = new List<string> { "X", "Y" }
        };
        var command = new UpdateMovieCommand(movieId, updateDto);

        // After update, the repository should return the updated entity
        _movieRepoMock
            .SetupSequence(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing)         // first fetch
            .ReturnsAsync(new Movie        // second fetch returns updated
            {
                Id = movieId,
                Title = updateDto.Title,
                Director = updateDto.Director,
                Producer = updateDto.Producer,
                ReleaseDate = updateDto.ReleaseDate,
                OpeningCrawl = updateDto.OpeningCrawl,
                Characters = updateDto.Characters
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: repository.Update and CommitAsync invoked
        _movieRepoMock.Verify(r => r.Update(It.Is<Movie>(m => m.Id == movieId && m.Title == updateDto.Title)), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Assert: returned DTO matches updated entity
        result.Should().BeEquivalentTo(new MovieDetailDto
        {
            Id = movieId,
            Title = updateDto.Title,
            Director = updateDto.Director,
            Producer = updateDto.Producer,
            ReleaseDate = updateDto.ReleaseDate,
            OpeningCrawl = updateDto.OpeningCrawl,
            Characters = updateDto.Characters
        });
    }

    /// <summary>
    /// Test to ensure that when a movie is not found, a NotFoundException is thrown.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMovieNotFound_ThrowsNotFoundException()
    {
        // Arrange: repository returns null initially
        var movieId = Guid.NewGuid();
        _movieRepoMock
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        var command = new UpdateMovieCommand(movieId, new UpdateMovieDto());

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert: first fetch missing triggers NotFoundException
        await act.Should()
                 .ThrowAsync<NotFoundException>()
                 .WithMessage($"Movie with Id {movieId} not found.");
        _movieRepoMock.Verify(r => r.Update(It.IsAny<Movie>()), Times.Never);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    /// Test to ensure that when a movie is found but missing after update,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMissingAfterUpdate_ThrowsNotFoundException()
    {
        // Arrange: initial fetch returns entity
        var movieId = Guid.NewGuid();
        var existing = new Movie { Id = movieId };
        _movieRepoMock
            .SetupSequence(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing)   // first fetch
            .ReturnsAsync((Movie)null); // second fetch

        var command = new UpdateMovieCommand(movieId, new UpdateMovieDto
        {
            Title = "T",
            Director = "D",
            Producer = "P",
            ReleaseDate = DateTime.Now,
            OpeningCrawl = "C",
            Characters = new List<string>()
        });

        // Act
        Func<Task> act = () => _handler.Handle(command, CancellationToken.None);

        // Assert: update called, but missing after update throws
        await act.Should()
                 .ThrowAsync<NotFoundException>()
                 .WithMessage($"Movie with Id {movieId} not found after update.");
        _movieRepoMock.Verify(r => r.Update(It.IsAny<Movie>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
