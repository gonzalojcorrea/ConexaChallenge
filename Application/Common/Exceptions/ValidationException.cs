using FluentValidation.Results;

namespace Application.Common.Exceptions;

/// <summary>
/// Represents errors that occur during validation.
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified validation failure collection.
    /// </summary>
    /// <param name="failures"></param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("One or more validation errors occurred.")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray()
            );
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationException"/> class with a specified dictionary of errors.
    /// </summary>
    /// <param name="errors"></param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors;
    }
}
