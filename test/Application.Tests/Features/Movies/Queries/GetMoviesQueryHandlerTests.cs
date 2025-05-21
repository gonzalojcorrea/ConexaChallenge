using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Features.Movies.Queries.GetMovies;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace Application.Tests.Features.Movies.Queries;

/// <summary>
/// Unit tests for GetMoviesQueryHandler.
/// </summary>
public class GetMoviesQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMovieRepository> _movieRepoMock;
    private readonly GetMoviesQueryHandler _handler;

    public GetMoviesQueryHandlerTests()
    {
        // Arrange common mocks
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _movieRepoMock = new Mock<IMovieRepository>();

        // Wire up unitOfWork.Movies to our movie repository mock
        _unitOfWorkMock
            .Setup(u => u.Movies)
            .Returns(_movieRepoMock.Object);

        // Instantiate the handler under test
        _handler = new GetMoviesQueryHandler(_unitOfWorkMock.Object);
    }

    /// <summary>
    /// Test to ensure that when movies exist, the correct list of MovieDto is returned.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenMoviesExist_ReturnsListOfMovieDtos()
    {
        // Arrange: repository returns two domain Movie entities
        var domainMovies = new List<Movie>
            {
                new Movie { Id = Guid.NewGuid(), Title = "A New Hope" },
                new Movie { Id = Guid.NewGuid(), Title = "Empire Strikes Back" }
            };
        _movieRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainMovies);

        var query = new GetMoviesQuery();

        // Act: execute the handler
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert: we got exactly two DTOs with the same Ids and Titles
        result.Should().HaveCount(2);
        result.Select(d => d.Id)
              .Should().BeEquivalentTo(domainMovies.Select(m => m.Id));
        result.Select(d => d.Title)
              .Should().BeEquivalentTo(domainMovies.Select(m => m.Title));
    }

    /// <summary>
    /// Test to ensure that when no movies exist, a NotFoundException is thrown.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Handle_WhenNoMoviesExist_ThrowsNotFoundException()
    {
        // Arrange: repository returns an empty list
        _movieRepoMock
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>());

        var query = new GetMoviesQuery();

        // Act: capture the call to Handle
        Func<Task> act = () => _handler.Handle(query, CancellationToken.None);

        // Assert: a NotFoundException with the expected message is thrown
        await act.Should()
                 .ThrowAsync<NotFoundException>()
                 .WithMessage("No movies found.");
    }
}
