// WebApi/Middleware/ExceptionHandlingMiddleware.cs
using System.Net;
using System.Text.Json;

namespace FlapKapVendingMachine.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); // Continue request pipeline
        }
        catch (Exception ex)
        {
            // Log detailed error with request info
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}",
                context.Request?.Method,
                context.Request?.Path.Value);

            // Return generic error to client
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var errorResponse = new
            {
                statusCode = context.Response.StatusCode,
                message = "An unexpected error occurred.",
                traceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(json);
        }
    }
}
