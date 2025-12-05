using System.Diagnostics;

namespace Gateway.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses.
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        _logger.LogInformation(
            "Incoming request: {Method} {Path}",
            requestMethod,
            requestPath);

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "Completed request: {Method} {Path} - Status: {StatusCode} - Duration: {Duration}ms",
            requestMethod,
            requestPath,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}


