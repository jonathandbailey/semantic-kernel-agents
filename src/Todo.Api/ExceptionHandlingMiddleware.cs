using Microsoft.AspNetCore.Mvc;

namespace Todo.Api;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private const string ResponseContentType = "application/json";
    private const string? AnErrorOccurredWhileProcessingYourRequest = "An error occurred while processing your request.";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred and has been handled at root (Middleware).");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            ArgumentNullException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetails
        {
            Type = $"https://httpstatuses.io/{statusCode}",
            Title = AnErrorOccurredWhileProcessingYourRequest,
            Status = statusCode,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = ResponseContentType;

        return context.Response.WriteAsJsonAsync(problem);
    }
}