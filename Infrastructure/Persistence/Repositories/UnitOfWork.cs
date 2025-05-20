using Application.Common.Interfaces;
using Domain.Interfaces;

namespace Infrastructure.Persistence.Repositories;

/// <summary>
/// Unit of Work implementation for managing repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IMovieRepository Movies { get; }
    public IUserRepository Users { get; }
    public IRoleRepository Roles { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;

        Movies = new MovieRepository(_context);
        Users = new UserRepository(_context);
        Roles = new RoleRepository(_context);
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<int> CommitAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    /// <summary>
    /// Disposes the Unit of Work and its resources.
    /// </summary>
    public void Dispose()
        => _context.Dispose();
}
