using Infrastructure.Configurations.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configurations.Extensions;

/// <summary>
/// Extension methods for configuring MVC services.
/// </summary>
public static class MvcBuilderExtensions
{
    /// <summary>
    /// Adds custom filters to the MVC builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddCustomFilters(this IMvcBuilder builder)
    {
        // Validate the ModelState and convert it to RFC 7807 format
        builder.ConfigureApiBehaviorOptions(opts =>
        {
            opts.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(kvp => kvp.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors
                        .Select(e => $"{kvp.Key}: {e.ErrorMessage}"))
                    .ToArray();

                var detail = string.Join(" | ", errors);

                var problem = new ProblemDetails
                {
                    Type = "https://api.conexa.com/errors/validation-error",
                    Title = "Validation Error",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = detail,
                    Instance = context.HttpContext.Request.Path
                };

                return new BadRequestObjectResult(problem)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        });

        // Add your SuccessResponseFilter to wrap 2xx responses
        builder.AddMvcOptions(opts =>
        {
            opts.Filters.Add<SuccessResponseFilter>();
        });

        return builder;
    }
}
