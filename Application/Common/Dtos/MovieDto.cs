namespace Application.Common.Dtos;

/// <summary>
/// Data Transfer Object for Movie.
/// </summary>
public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; init; }
}
