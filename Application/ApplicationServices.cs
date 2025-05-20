using Application.Behaviors;
using Domain.Entities;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
/// Extension methods for adding application services.
/// </summary>
public static class ApplicationServices
{
    /// <summary>
    /// Adds application services to the specified service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // 1. MediatR (handlers, validators…)
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(ApplicationServices).Assembly));

        // 2. FluentValidation: registra todos los IValidator<T> del ensamblado
        services.AddValidatorsFromAssembly(typeof(ApplicationServices).Assembly);

        // 3. Pipeline: inyecta el ValidationBehavior para que MediatR valide antes de cada handler
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // 4. Password Hasher
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}
