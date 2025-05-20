namespace Application.Common.Exceptions;

/// <summary>
/// Represents a Bad Request error (400 Bad Request).
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message) { }
}