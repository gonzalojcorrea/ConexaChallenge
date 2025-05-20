using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interface for the Movie repository.
/// </summary>
public interface IMovieRepository
{
    /// <summary>
    /// Adds a new movie to the repository.
    /// </summary>
    /// <param name="movie"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(Movie movie, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a movie by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a movie by its title.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Movie?> GetByTitleAsync(string title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all movies in the repository.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Update a movie from the repository.
    /// </summary>
    /// <param name="movie"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public void Update(Movie movie);
}
