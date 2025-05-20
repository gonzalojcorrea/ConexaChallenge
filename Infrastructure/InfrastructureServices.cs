using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Configurations.Authentication;
using Infrastructure.Configurations.Extensions;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure;

/// <summary>
/// Extension methods for adding infrastructure services.
/// </summary>
public static class InfrastructureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, IConfiguration configuration)
    {
        // 1. EF Core + DbContext/UoW/Repos
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMovieRepository, MovieRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();

        // 2. JWT settings + generador
        services.Configure<JwtSettings>(
            s => configuration.GetSection(JwtSettings.SectionName).Bind(s));
        services.AddSingleton<IJwtService, JwtService>();

        // 3. Bind JwtSettings
        var section = configuration.GetSection(JwtSettings.SectionName);
        var jwt = section.Get<JwtSettings>();

        // 4. Configure Authentication & JWT Bearer
        services.AddAuthenticationExtension(configuration, jwt);

        // 5. Configure Authorization
        services.AddAuthorization();

        return services;
    }
}
