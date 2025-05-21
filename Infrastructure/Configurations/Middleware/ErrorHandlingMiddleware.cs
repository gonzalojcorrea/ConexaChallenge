using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using Polly.Timeout;
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

    /// <summary>
    /// Handles exceptions thrown in the pipeline.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
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
        // 4. Circuit breaker open → 503 Service Unavailable
        catch (BrokenCircuitException ex)
        {
            _logger.LogWarning(ex, "Circuit breaker is open, external service unavailable");

            var problem = new ProblemDetails
            {
                Type = "https://api.conexa.com/errors/service-unavailable",
                Title = "Service Unavailable",
                Status = (int)HttpStatusCode.ServiceUnavailable,
                Detail = "External service is temporarily unavailable. Please try again later."
            };

            await WriteProblemAsync(context, problem);
        }
        // 5. Timeout → 504 Gateway Timeout
        catch (TimeoutRejectedException ex)
        {
            _logger.LogWarning(ex, "Request to external service timed out");

            var problem = new ProblemDetails
            {
                Type = "https://api.conexa.com/errors/gateway-timeout",
                Title = "Gateway Timeout",
                Status = (int)HttpStatusCode.GatewayTimeout,
                Detail = "Request to external service timed out."
            };

            await WriteProblemAsync(context, problem);
        }
        // 6. HTTP failures → 502 Bad Gateway
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to external service failed");

            var problem = new ProblemDetails
            {
                Type = "https://api.conexa.com/errors/bad-gateway",
                Title = "Bad Gateway",
                Status = (int)HttpStatusCode.BadGateway,
                Detail = ex.Message
            };

            await WriteProblemAsync(context, problem);
        }
        // 7. Fallback → 500 Internal Server Error
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");

            var problem = new ProblemDetails
            {
                Type = "https://api.conexa.com/errors/internal-server-error",
                Title = "Internal Server Error",
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
