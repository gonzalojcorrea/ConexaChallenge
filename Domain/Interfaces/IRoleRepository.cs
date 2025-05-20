using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Repository interface for managing roles.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Adds a new role to the repository.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Role?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a role by its name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}
