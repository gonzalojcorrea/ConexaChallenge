namespace Application.Common.Models;

public class ErrorResponse
{
    public string Type { get; init; }
    public string Title { get; init; }
    public int StatusCode { get; init; }
    public string Detail { get; init; }
}
