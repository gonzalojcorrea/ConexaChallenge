using FluentValidation;

namespace Application.Features.Movies.Commands.CreateMovie;

/// <summary>
/// Validator for the CreateMovieCommand.
/// </summary>
public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Título obligatorio");
        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Fecha de estreno no puede ser futura");
    }
}
