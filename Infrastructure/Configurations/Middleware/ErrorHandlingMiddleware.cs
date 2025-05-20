using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Configurations.Middleware;

/// <summary>
/// Middleware to handle exceptions globally.
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

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(ctx, ex);
        }
    }

    /// <summary>
    /// Handles exceptions and returns a standardized error response (RFC 7807).
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    private Task HandleExceptionAsync(HttpContext ctx, Exception ex)
    {
        var pd = new ProblemDetails
        {
            Instance = ctx.Request.Path
        };

        switch (ex)
        {
            case BadRequestException br:
                pd.Type = "https://api.conexa.com/errors/bad-request";
                pd.Title = "Bad Request";
                pd.Status = StatusCodes.Status400BadRequest;
                pd.Detail = br.Message;
                break;

            case NotFoundException nf:
                pd.Type = "https://api.conexa.com/errors/not-found";
                pd.Title = "Not Found";
                pd.Status = StatusCodes.Status404NotFound;
                pd.Detail = nf.Message;
                break;

            default:
                _logger.LogError(ex, "Unhandled exception");
                pd.Type = "https://api.conexa.com/errors/internal-server-error";
                pd.Title = "Internal Server Error";
                pd.Status = StatusCodes.Status500InternalServerError;
                pd.Detail = "An unexpected error occurred.";
                break;
        }

        ctx.Response.ContentType = "application/problem+json";
        ctx.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;

        return ctx.Response.WriteAsJsonAsync(pd);
    }
}
