using System.Text.Json.Serialization;

namespace Application.Common.Models;

public class MovieApiModel
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("director")]
    public string Director { get; set; } = default!;

    [JsonPropertyName("producer")]
    public string Producer { get; set; } = default!;

    [JsonPropertyName("release_date")]
    public DateTime ReleaseDate { get; set; }

    [JsonPropertyName("opening_crawl")]
    public string OpeningCrawl { get; set; } = default!;

    [JsonPropertyName("characters")]
    public IEnumerable<string> Characters { get; set; } = default!;
}

