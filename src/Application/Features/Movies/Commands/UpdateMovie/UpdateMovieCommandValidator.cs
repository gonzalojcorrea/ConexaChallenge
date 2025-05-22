using FluentValidation;

namespace Application.Features.Movies.Commands.UpdateMovie;

/// <summary>
/// Validator for the UpdateMovieCommand.
/// </summary>
public class UpdateMovieCommandValidator : AbstractValidator<UpdateMovieCommand>
{
    public UpdateMovieCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Movie Id is required.");

        RuleFor(x => x.Data)
            .NotNull()
            .WithMessage("Update data is required.");

        When(x => x.Data != null, () =>
        {
            RuleFor(x => x.Data.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(200)
                .WithMessage("Title must be at most 200 characters.");

            RuleFor(x => x.Data.Director)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Description must be at most 1000 characters.");

            RuleFor(x => x.Data.Producer)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Producer must be at most 100 characters.");

            RuleFor(x => x.Data.ReleaseDate)
                .NotEmpty()
                .WithMessage("Release date is required.")
                .Must(date => date != default)
                .WithMessage("Invalid release date.");

            RuleFor(x => x.Data.OpeningCrawl)
                .NotEmpty()
                .MaximumLength(2000)
                .WithMessage("Opening crawl must be at most 2000 characters.");

            RuleFor(x => x.Data.Characters)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("La lista de personajes es obligatoria.")
                .Must(x => x.Count <= 20).WithMessage("No puede haber más de 20 personajes");
        });
    }
}
