using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Infrastructure.Configurations.Filters;

/// <summary>
/// A filter that wraps successful responses in a standardized format.
/// </summary>
public class SuccessResponseFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context) { }

    /// <summary>
    /// Executes after the action method has been called.
    /// </summary>
    /// <param name="context"></param>
    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Result is ObjectResult result
         && result.StatusCode is int code
         && code >= 200 && code < 300)
        {
            var payload = result.Value;
            var wrapperType = typeof(SuccessResponse<>)
                              .MakeGenericType(payload?.GetType() ?? typeof(object));
            var wrapper = Activator.CreateInstance(wrapperType);

            wrapperType.GetProperty("Title")?
                .SetValue(wrapper, GetTitle(code));
            wrapperType.GetProperty("StatusCode")?
                .SetValue(wrapper, code);
            wrapperType.GetProperty("Detail")?
                .SetValue(wrapper, GetDetail(code));
            wrapperType.GetProperty("Data")?
                .SetValue(wrapper, payload);

            context.Result = new ObjectResult(wrapper)
            {
                StatusCode = code
            };
        }
    }

    /// <summary>
    /// Generates a title based on the HTTP status code.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    private static string GetTitle(int statusCode) => statusCode switch
    {
        (int)HttpStatusCode.OK => "OK",
        (int)HttpStatusCode.Created => "Created",
        (int)HttpStatusCode.NoContent => "No Content",
        _ => $"Status {statusCode}"
    };

    /// <summary>
    /// Generates a detail message based on the HTTP status code.
    /// </summary>
    /// <param name="statusCode"></param>
    /// <returns></returns>
    private static string GetDetail(int statusCode) => statusCode switch
    {
        (int)HttpStatusCode.OK => "Request succeeded.",
        (int)HttpStatusCode.Created => "Resource created successfully.",
        (int)HttpStatusCode.NoContent => "No content to return.",
        _ => string.Empty
    };
}
