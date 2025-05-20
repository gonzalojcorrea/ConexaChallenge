using Infrastructure.Configurations.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Infrastructure.Configurations.Extensions;

/// <summary>
/// Extension methods for adding authentication services.
/// </summary>
public static class AuthenticationExtension
{
    /// <summary>
    /// Extension method to add JWT authentication to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="jwt"></param>
    /// <returns></returns>
    public static IServiceCollection AddAuthenticationExtension(this IServiceCollection services, IConfiguration configuration, JwtSettings jwt)
    {
        // 1. Configure JWT settings
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            // 1.1. Bind JwtSettings
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                                              Encoding.UTF8.GetBytes(jwt.SecretKey)
                                          ),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // 1.2. Configure JWT Bearer events
            opt.Events = new JwtBearerEvents
            {
                OnChallenge = async ctx =>
                {
                    ctx.HandleResponse();
                        
                    var pd = new ProblemDetails
                    {
                        Type = "https://api.conexa.com/errors/unauthorized",
                        Title = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Detail = "Authentication is required to access this resource.",
                        Instance = ctx.Request.Path
                    };

                    ctx.Response.StatusCode = pd.Status.Value;
                    ctx.Response.ContentType = "application/problem+json";
                    await ctx.Response.WriteAsJsonAsync(pd);
                },
                OnForbidden = async ctx =>
                {
                    var pd = new ProblemDetails
                    {
                        Type = "https://api.conexa.com/errors/forbidden",
                        Title = "Forbidden",
                        Status = StatusCodes.Status403Forbidden,
                        Detail = "You do not have permission to access this resource.",
                        Instance = ctx.Request.Path
                    };

                    ctx.Response.StatusCode = pd.Status.Value;
                    ctx.Response.ContentType = "application/problem+json";
                    await ctx.Response.WriteAsJsonAsync(pd);
                }
            };
        });

        return services;
    }
}
