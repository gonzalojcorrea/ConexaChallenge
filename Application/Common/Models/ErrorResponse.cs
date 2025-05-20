namespace Application.Common.Models;

public class ErrorResponse
{
    public string Type { get; init; }
    public string Title { get; init; }
    public int Status { get; init; }
    public string Detail { get; init; }
    public string Instance { get; init; }
}
