using System.Net;
using System.Text.Json;

namespace Todo.Api;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
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
        var code = HttpStatusCode.InternalServerError;

        var result = JsonSerializer.Serialize(new { error = exception.Message });
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            _ => (int)code
        };

        return context.Response.WriteAsync(result);
    }
}