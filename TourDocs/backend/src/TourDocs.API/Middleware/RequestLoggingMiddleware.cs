using System.Diagnostics;

namespace TourDocs.API.Middleware;

/// <summary>
/// Middleware that logs request and response details including timing information.
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
        var correlationId = Guid.NewGuid().ToString("N")[..12];
        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Append("X-Correlation-Id", correlationId);

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "[{CorrelationId}] {Method} {Path} started",
            correlationId, context.Request.Method, context.Request.Path);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var level = stopwatch.ElapsedMilliseconds > 500
                ? LogLevel.Warning
                : LogLevel.Information;

            _logger.Log(level,
                "[{CorrelationId}] {Method} {Path} completed {StatusCode} in {Elapsed}ms",
                correlationId,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
