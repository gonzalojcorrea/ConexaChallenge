namespace Application.Common.Models;

public class SuccessResponse<T>
{
    /// <summary>The title of the response, e.g. "OK", "Created".</summary>
    public string Title { get; init; }

    /// <summary>The HTTP status code, e.g. 200, 201.</summary>
    public int StatusCode { get; init; }

    /// <summary>Detail message for the client.</summary>
    public string Detail { get; init; }

    /// <summary>The actual payload.</summary>
    public T Data { get; init; }
}
