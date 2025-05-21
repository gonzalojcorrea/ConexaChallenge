using Domain.Entities;

namespace Application.Common.Interfaces;

/// <summary>
/// Interface for JWT service.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT token for the given user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    string GenerateToken(User user);
}
