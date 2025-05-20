using Domain.Entities;
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

        // 2. Password Hasher
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        return services;
    }
}
