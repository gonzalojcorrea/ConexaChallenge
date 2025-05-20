using Infrastructure.Configurations.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Configurations.Extensions;

/// <summary>
/// Extension methods for configuring middlewares.
/// </summary>
public static class MiddlewareExtension
{
    /// <summary>
    /// Configures the middlewares for the application.
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder ConfigureMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }
}
