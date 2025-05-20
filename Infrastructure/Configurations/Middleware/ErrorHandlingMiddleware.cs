using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace Infrastructure.Configurations.Middleware;

/// <summary>
/// Global exception handler middleware producing RFC7807-compliant responses.
/// </summary>
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        // 1. FluentValidation failures → 400
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed in pipeline");

            // Flatten errors into a single string: "Field1: msg1, Field1: msg2, Field2: msg3"
            var errorsString = string.Join("; ",
                ex.Errors
                  .SelectMany(kvp => kvp.Value.Select(msg => $"{kvp.Key}: {msg}"))
            );

            var problem = new ValidationProblemDetails(ex.Errors)
            {
                Type = "https://api.conexa.com/errors/bad-request",
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest,
                Detail = errorsString
            };

            await WriteProblemAsync(context, problem);
        }
        // 2. BadRequestException → 400
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Bad request");

            var problem = new ProblemDetails
            {
                Type = "https://api.conexa.com/errors/bad-request",
                Title = "Bad Request",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = ex.Message
            };

            await WriteProblemAsync(context, problem);
        }
        // 3. NotFoundException → 404
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found");

            var problem = new ProblemDetails
            {
                Type = "https://api.conexa.com/errors/not-found",
                Title = "Not Found",
                Status = (int)HttpStatusCode.NotFound,
                Detail = ex.Message
            };

            await WriteProblemAsync(context, problem);
        }
        // 4. Fallback → 500
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");

            var problem = new ProblemDetails
            {
                Type = "https://api.conexa.com/errors/internal-server-error",
                Title = ex.Message,
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = ex.ToString()
            };

            await WriteProblemAsync(context, problem);
        }
    }

    /// <summary>
    /// Serialize ProblemDetails or ValidationProblemDetails to RFC7807 response.
    /// </summary>
    private static Task WriteProblemAsync(HttpContext context, ProblemDetails problem)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problem);
        return context.Response.WriteAsync(json);
    }
}
