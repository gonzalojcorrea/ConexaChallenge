namespace Application.Common.Models;

/// <summary>
/// Represents an error response model.
/// </summary>
public class ErrorResponse
{
    /// <summary>Url to the documentation page for this error.</summary>
    public string Type { get; init; }

    /// <summary>The title of the response indicating the error type.</summary>
    public string Title { get; init; }

    /// <summary>The Error HTTP status code.</summary>
    public int StatusCode { get; init; }

    /// <summary>Gets or sets the error message.</summary>
    public string Detail { get; init; }
}
