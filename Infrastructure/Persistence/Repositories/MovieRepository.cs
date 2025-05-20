using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository for managing movies.
/// </summary>
public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _context;
    public MovieRepository(AppDbContext context) => _context = context;

    /// <summary>
    /// Adds a new movie to the database.
    /// </summary>
    /// <param name="movie"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task AddAsync(Movie movie, CancellationToken ct = default)
        => await _context.Movies.AddAsync(movie, ct);

    /// <summary>
    /// Gets a movie by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.Movies.FindAsync(id, ct);

    /// <summary>
    /// Retrieves a movie by its title.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Movie?> GetByTitleAsync(string title, CancellationToken ct = default)
        => await _context.Movies.FirstOrDefaultAsync(m => m.Title == title, ct);

    /// <summary>
    /// Retrieves all movies from the database.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken ct = default)
        => await _context.Movies.AsNoTracking().ToListAsync(ct);

    /// <summary>
    /// Updates an existing movie in the database.
    /// </summary>
    /// <param name="movie"></param>
    public void Update(Movie movie) 
        => _context.Movies.Update(movie);
}
