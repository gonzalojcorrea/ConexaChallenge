using Domain.Interfaces;

namespace Application.Common.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IMovieRepository Movies { get; }
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
