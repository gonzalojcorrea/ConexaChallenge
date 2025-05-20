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
        var api = await _http.GetFromJsonAsync<SwapiResponse<MovieApiModel>>("films");
        return api?.Result.Select(r => r.Properties) ?? Enumerable.Empty<MovieApiModel>();
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