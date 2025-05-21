using Application.Common.Interfaces;
using Application.Common.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

/// <summary>
/// Implementation of the StarWarsAPI client.
/// </summary>
public class SwapiClient : ISwapiClient
{
    private readonly HttpClient _http;
    public SwapiClient(HttpClient http) => _http = http;

    /// <summary>
    /// Fetches all films from the StarWarsAPI and resolves character names.
    /// </summary>
    /// <returns></returns>
    public async Task<IEnumerable<MovieApiModel>> GetAllFilmsAsync()
    {
        // 1. Fetch films
        var filmsResp = await _http.GetFromJsonAsync<SwapiResponse<MovieApiModel>>("films");
        if (filmsResp?.Result == null) return Enumerable.Empty<MovieApiModel>();

        // 2. Fetch all people once
        var peopleList = await _http.GetFromJsonAsync<PeopleListResponse>("people?page=1&limit=100");
        var charNameMap = peopleList?.Results
            .ToDictionary(p => p.Url, p => p.Name) ?? new Dictionary<string, string>();

        // 3. Map films with resolved character names
        return filmsResp.Result.Select(r => r.Properties)
            .Select(props => new MovieApiModel
            {
                Title = props.Title,
                Director = props.Director,
                Producer = props.Producer,
                ReleaseDate = props.ReleaseDate,
                OpeningCrawl = props.OpeningCrawl,
                Characters = props.Characters
                    .Select(url => charNameMap.TryGetValue(url, out var name) ? name : url)
                    .ToList()
            });
    }
}

/// <summary>
/// Generic class to represent a SWAPI entity.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SwapiEntity<T>
{
    [JsonPropertyName("properties")]
    public T Properties { get; set; } = default!;
}

/// <summary>
/// Generic class to represent a SWAPI response.
/// </summary>
/// <typeparam name="T"></typeparam>
public class SwapiResponse<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    [JsonPropertyName("result")]
    public SwapiEntity<T>[] Result { get; set; } = default!;
}

/// <summary>
/// Class to represent a person in the SWAPI.
/// </summary>
public class PersonListResult
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}

/// <summary>
/// Class to represent a list string of people in the SWAPI.
/// </summary>
public class PeopleListResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    [JsonPropertyName("results")]
    public PersonListResult[] Results { get; set; } = default!;
}
