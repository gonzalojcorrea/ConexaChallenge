using Application.Common.Interfaces;
using Application.Common.Models;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.Services;

public class SwapiClient : ISwapiClient
{
    private readonly HttpClient _http;
    public SwapiClient(HttpClient http) => _http = http;

    public async Task<IEnumerable<MovieApiModel>> GetAllFilmsAsync()
    {
        // Fetch films
        var filmsResp = await _http.GetFromJsonAsync<SwapiResponse<MovieApiModel>>("films");
        if (filmsResp?.Result == null) return Enumerable.Empty<MovieApiModel>();

        // Fetch all people once
        var peopleList = await _http.GetFromJsonAsync<PeopleListResponse>("people?page=1&limit=100");
        var charNameMap = peopleList?.Results
            .ToDictionary(p => p.Url, p => p.Name) ?? new Dictionary<string, string>();

        // Map films with resolved character names
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

public class SwapiEntity<T>
{
    [JsonPropertyName("properties")]
    public T Properties { get; set; } = default!;
}

public class SwapiResponse<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    [JsonPropertyName("result")]
    public SwapiEntity<T>[] Result { get; set; } = default!;
}

// People list response
public class PersonListResult
{
    [JsonPropertyName("uid")]
    public string Uid { get; set; } = default!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}

public class PeopleListResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = default!;

    [JsonPropertyName("results")]
    public PersonListResult[] Results { get; set; } = default!;
}
