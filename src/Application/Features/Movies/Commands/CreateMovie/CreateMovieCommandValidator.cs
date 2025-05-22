using FluentValidation;

namespace Application.Features.Movies.Commands.CreateMovie;

/// <summary>
/// Validator for the CreateMovieCommand.
/// </summary>
public class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Título obligatorio")
            .MaximumLength(200).WithMessage("Título no puede ser mayor a 100 caracteres");

        RuleFor(x => x.Director)
            .NotEmpty()
            .WithMessage("Director obligatorio")
            .MaximumLength(100)
            .WithMessage("Director no puede ser mayor a 100 caracteres");

        RuleFor(x => x.Producer)
            .NotEmpty()
            .WithMessage("Productor obligatorio")
            .MaximumLength(100)
            .WithMessage("Productor no puede ser mayor a 100 caracteres");

        RuleFor(x => x.ReleaseDate)
            .NotEmpty();

        RuleFor(x => x.OpeningCrawl)
            .NotEmpty()
            .MaximumLength(2000).WithMessage("OpeningCrawl no puede ser mayor a 1000 caracteres");

        RuleFor(x => x.Characters)
            .NotEmpty()
            .Must(x => x.Count <= 20)
            .WithMessage("No puede haber más de 20 personajes");
    }
}
