using FluentValidation;
using MediatR;

namespace Application.Behaviors;

/// <summary>
/// Validation behavior for MediatR requests.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    /// <summary>
    /// Handles the validation of the request.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Common.Exceptions.ValidationException"></exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .Select(f => new FluentValidation.Results.ValidationFailure(
                f.PropertyName, f.ErrorMessage))
            .ToList();

        if (failures.Any())
            throw new Common.Exceptions.ValidationException(failures);

        return await next();
    }
}
