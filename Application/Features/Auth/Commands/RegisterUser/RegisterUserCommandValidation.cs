﻿using FluentValidation;

namespace Application.Features.Auth.Commands.RegisterUser;

public class RegisterUserCommandValidation : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidation()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("El nombre de usuario es obligatorio.")
            .MinimumLength(3)
            .WithMessage("El nombre de usuario debe tener al menos 3 caracteres.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria.")
            .MinimumLength(6)
            .WithMessage("La contraseña debe tener al menos 6 caracteres.");

        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage("El rol es obligatorio.");
    }
}
