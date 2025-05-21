using Domain.Entities;

namespace Domain.Interfaces;

/// <summary>
/// Interface for user repository.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adds a new user to the repository.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}
