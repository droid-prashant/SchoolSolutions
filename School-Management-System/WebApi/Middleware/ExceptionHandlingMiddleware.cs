using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception while processing request {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteErrorResponseAsync(context, exception);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        if (context.Response.HasStarted)
        {
            throw exception;
        }

        context.Response.Clear();
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)GetStatusCode(exception);

        var payload = new
        {
            message = GetClientMessage(exception)
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static HttpStatusCode GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            InvalidOperationException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            DbUpdateConcurrencyException => HttpStatusCode.Conflict,
            _ => HttpStatusCode.InternalServerError
        };
    }

    private static string GetClientMessage(Exception exception)
    {
        return string.IsNullOrWhiteSpace(exception.Message)
            ? "Something went wrong while processing the request."
            : exception.Message;
    }
}
