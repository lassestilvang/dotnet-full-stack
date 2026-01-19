using System.Diagnostics;

namespace InventoryHub.Api.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses.
/// Tracks request duration for performance monitoring.
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
        var requestId = Guid.NewGuid().ToString("N")[..8];

        // Log incoming request
        _logger.LogInformation(
            "[{RequestId}] {Method} {Path} - Started",
            requestId,
            context.Request.Method,
            context.Request.Path);

        try
        {
            await _next(context);

            stopwatch.Stop();

            // Log completed request with duration
            _logger.LogInformation(
                "[{RequestId}] {Method} {Path} - Completed {StatusCode} in {Duration}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log failed request
            _logger.LogError(
                ex,
                "[{RequestId}] {Method} {Path} - Failed after {Duration}ms",
                requestId,
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}

/// <summary>
/// Extension methods for registering middleware.
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
