using FluentValidation;

namespace Application.Features.Auth.Queries.LoginUser;

/// <summary>
/// Validator for the LoginUserQuery.
/// </summary>
public class LoginUserQueryValidation : AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidation()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("El nombre de usuario es obligatorio.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria.");
    }
}
