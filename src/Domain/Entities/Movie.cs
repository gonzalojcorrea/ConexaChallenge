namespace Domain.Entities;

/// <summary>
/// Represents a movie entity.
/// </summary>
public class Movie : BaseEntity
{
    public string Title { get; set; }
    public string Director { get; set; }
    public string Producer { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string OpeningCrawl { get; set; }
    public List<string> Characters { get; set; }
}
