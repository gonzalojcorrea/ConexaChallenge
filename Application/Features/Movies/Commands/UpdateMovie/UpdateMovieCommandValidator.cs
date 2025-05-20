using FluentValidation;

namespace Application.Features.Movies.Commands.UpdateMovie;

/// <summary>
/// Validator for the UpdateMovieCommand.
/// </summary>
public class UpdateMovieCommandValidator
        : AbstractValidator<UpdateMovieCommand>
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
                .MaximumLength(50)
                .WithMessage("Description must be at most 1000 characters.");

            RuleFor(x => x.Data.ReleaseDate)
                .NotEmpty()
                .WithMessage("Release date is required.")
                .Must(date => date != default)
                .WithMessage("Invalid release date.");
        });
    }
}
