using FluentValidation;

namespace Application.Features.Auth.Queries.LoginUser;

/// <summary>
/// Validator for the LoginUserQuery.
/// </summary>
public class LoginUserQueryValidation : AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidation()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El correo electrónico es obligatorio.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria.");
    }
}
