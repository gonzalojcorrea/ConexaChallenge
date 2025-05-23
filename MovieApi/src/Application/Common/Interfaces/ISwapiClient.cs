using Application.Common.Models;

namespace Application.Common.Interfaces;

/// <summary>
/// Interface for the StarWarsAPI client.
/// </summary>
public interface ISwapiClient
{
    /// <summary>
    /// Fetches all films from the StarWarsAPI.
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<MovieApiModel>> GetAllFilmsAsync();
}
