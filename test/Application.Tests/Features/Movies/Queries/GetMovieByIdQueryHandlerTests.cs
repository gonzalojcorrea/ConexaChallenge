using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Movies.Queries.GetMovieById;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Movies.Queries;

/// <summary>
/// Unit tests for GetMovieByIdQueryHandler.
/// </summary>
public class GetMovieByIdQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMovieRepository> _movieRepoMock;
    private readonly GetMovieByIdQueryHandler _handler;

    public GetMovieByIdQueryHandlerTests()
    {
        // Arrange common mocks
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _movieRepoMock = new Mock<IMovieRepository>();

        // Wire up unitOfWork.Movies
        _unitOfWorkMock
            .Setup(u => u.Movies)
            .Returns(_movieRepoMock.Object);

        // Instantiate handler
        _handler = new GetMovieByIdQueryHandler(_unitOfWorkMock.Object);
    }

    /// <summary>
    /// Test to ensure that when a movie exists, the correct MovieDetailDto is returned.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMovieExists_ReturnsMovieDetailDto()
    {
        // Arrange: create a sample Movie
        var movieId = Guid.NewGuid();
        var domainMovie = new Movie
        {
            Id = movieId,
            Title = "Test Title",
            Director = "Test Director",
            Producer = "Test Producer",
            ReleaseDate = new DateTime(2025, 5, 21),
            OpeningCrawl = "Test Crawl",
            Characters = new List<string> { "Char1", "Char2" }
        };

        // Setup repository to return the movie
        _movieRepoMock
            .Setup(r => r.GetByIdAsync(movieId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainMovie);

        var query = new GetMovieByIdQuery(movieId);

        // Act: execute handler
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert: returned DTO matches domain entity
        result.Should().NotBeNull();
        result.Id.Should().Be(domainMovie.Id);
        result.Title.Should().Be(domainMovie.Title);
        result.Director.Should().Be(domainMovie.Director);
        result.Producer.Should().Be(domainMovie.Producer);
        result.ReleaseDate.Should().Be(domainMovie.ReleaseDate);
        result.OpeningCrawl.Should().Be(domainMovie.OpeningCrawl);
        result.Characters.Should().BeEquivalentTo(domainMovie.Characters);
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

        var query = new GetMovieByIdQuery(movieId);

        // Act: capture call
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert: NotFoundException with correct message
        await act.Should()
                 .ThrowAsync<NotFoundException>()
                 .WithMessage($"Movie with Id {movieId} not found.");
    }
}
