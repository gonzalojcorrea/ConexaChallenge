using FluentValidation.Results;

namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
/// <param name="errors"></param>
public class ValidationException(IEnumerable<ValidationFailure> errors) : Exception
{
    public IEnumerable<ValidationFailure> Errors { get; } = errors;
}
