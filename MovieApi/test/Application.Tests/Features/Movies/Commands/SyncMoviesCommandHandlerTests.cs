using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Movies.Commands.SyncMovies;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;

namespace Application.Tests.Features.Movies.Commands;

/// <summary>
/// Unit tests for SyncMoviesCommandHandler.
/// </summary>
public class SyncMoviesCommandHandlerTests
{
    private readonly Mock<ISwapiClient> _swapiMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IMovieRepository> _movieRepoMock;
    private readonly SyncMoviesCommandHandler _handler;

    public SyncMoviesCommandHandlerTests()
    {
        // Arrange common mocks
        _swapiMock = new Mock<ISwapiClient>();
        _uowMock = new Mock<IUnitOfWork>();
        _movieRepoMock = new Mock<IMovieRepository>();

        // Wire up unitOfWork.Movies and CommitAsync
        _uowMock.Setup(u => u.Movies).Returns(_movieRepoMock.Object);
        _uowMock.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(1);

        // Instantiate handler under test
        _handler = new SyncMoviesCommandHandler(_swapiMock.Object, _uowMock.Object);
    }

    /// <summary>
    /// Test to ensure that when no movies exist in the database,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenNoExistingMovies_AddsAllFilmsAndCommits()
    {
        // Arrange: SWAPI returns two films
        var apiFilms = new List<MovieApiModel>
            {
                new MovieApiModel
                {
                    Title = "Film A",
                    Director = "Dir A",
                    Producer = "Prod A",
                    ReleaseDate = new DateTime(2025,1,1),
                    OpeningCrawl = "Crawl A",
                    Characters = new[] { "C1", "C2" }
                },
                new MovieApiModel
                {
                    Title = "Film B",
                    Director = "Dir B",
                    Producer = "Prod B",
                    ReleaseDate = new DateTime(2025,2,2),
                    OpeningCrawl = "Crawl B",
                    Characters = new[] { "C3" }
                }
            };
        _swapiMock
            .Setup(s => s.GetAllFilmsAsync())
            .ReturnsAsync(apiFilms);

        // No existing movies in DB
        _movieRepoMock
            .Setup(r => r.GetByTitleAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);

        // Act
        var result = await _handler.Handle(new SyncMoviesCommand(), CancellationToken.None);

        // Assert: AddAsync called for each film
        _movieRepoMock.Verify(
            r => r.AddAsync(
                It.Is<Movie>(m => apiFilms.Any(f => f.Title == m.Title && f.Director == m.Director)),
                It.IsAny<CancellationToken>()),
            Times.Exactly(apiFilms.Count));

        // Commit once
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.Should().Be(Unit.Value);
    }

    /// <summary>
    /// Test to ensure that when movies exist in the database,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMoviesExist_UpdatesExistingAndCommitsWithoutAdding()
    {
        // Arrange: SWAPI returns one film
        var apiFilm = new MovieApiModel
        {
            Title = "Existing Film",
            Director = "New Dir",
            Producer = "New Prod",
            ReleaseDate = new DateTime(2025, 3, 3),
            OpeningCrawl = "New Crawl",
            Characters = new[] { "X", "Y", "Z" }
        };
        _swapiMock
            .Setup(s => s.GetAllFilmsAsync())
            .ReturnsAsync(new[] { apiFilm });

        // Existing movie in DB with old values
        var existing = new Movie
        {
            Id = Guid.NewGuid(),
            Title = apiFilm.Title,
            Director = "Old Dir",
            Producer = "Old Prod",
            ReleaseDate = new DateTime(2000, 1, 1),
            OpeningCrawl = "Old Crawl",
            Characters = new List<string> { "A" }
        };
        _movieRepoMock
            .Setup(r => r.GetByTitleAsync(apiFilm.Title, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(new SyncMoviesCommand(), CancellationToken.None);

        // Assert: No AddAsync calls
        _movieRepoMock.Verify(r => r.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()), Times.Never);

        // Commit once
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);

        // Existing entity's properties updated
        existing.Director.Should().Be(apiFilm.Director);
        existing.Producer.Should().Be(apiFilm.Producer);
        existing.ReleaseDate.Should().Be(apiFilm.ReleaseDate);
        existing.OpeningCrawl.Should().Be(apiFilm.OpeningCrawl);
        existing.Characters.Should().BeEquivalentTo(apiFilm.Characters);

        result.Should().Be(Unit.Value);
    }

    /// <summary>
    /// Test to ensure that when both existing and new films are present,
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMixedExistingAndNew_AddsAndUpdatesAppropriately()
    {
        // Arrange: two films, one new, one existing
        var newFilm = new MovieApiModel
        {
            Title = "NewFilm",
            Director = "DirN",
            Producer = "ProdN",
            ReleaseDate = new DateTime(2025, 4, 4),
            OpeningCrawl = "CrawlN",
            Characters = new[] { "N1" }
        };
        var existingFilmModel = new MovieApiModel
        {
            Title = "ExistFilm",
            Director = "DirE",
            Producer = "ProdE",
            ReleaseDate = new DateTime(2025, 5, 5),
            OpeningCrawl = "CrawlE",
            Characters = new[] { "E1", "E2" }
        };
        var films = new[] { newFilm, existingFilmModel };
        _swapiMock
            .Setup(s => s.GetAllFilmsAsync())
            .ReturnsAsync(films);

        // Setup existing for existingFilmModel
        var existing = new Movie { Id = Guid.NewGuid(), Title = existingFilmModel.Title, Characters = new List<string>() };
        _movieRepoMock
            .Setup(r => r.GetByTitleAsync(newFilm.Title, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie)null);
        _movieRepoMock
            .Setup(r => r.GetByTitleAsync(existingFilmModel.Title, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        // Act
        var result = await _handler.Handle(new SyncMoviesCommand(), CancellationToken.None);

        // Assert: Added once, updated once
        _movieRepoMock.Verify(r => r.AddAsync(It.Is<Movie>(m => m.Title == newFilm.Title), It.IsAny<CancellationToken>()), Times.Once);
        existing.Director.Should().Be(existingFilmModel.Director);
        existing.Producer.Should().Be(existingFilmModel.Producer);
        existing.ReleaseDate.Should().Be(existingFilmModel.ReleaseDate);
        existing.OpeningCrawl.Should().Be(existingFilmModel.OpeningCrawl);
        existing.Characters.Should().BeEquivalentTo(existingFilmModel.Characters);

        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        result.Should().Be(Unit.Value);
    }
}
