namespace Application.Common.Exceptions;

/// <summary>
/// Represents a Not Found error (404 Not Found).
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
