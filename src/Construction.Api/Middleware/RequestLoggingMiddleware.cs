using System.Diagnostics;

namespace Construction.Api.Middleware;

public sealed partial class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        long startedTimestamp = Stopwatch.GetTimestamp();

        await next(context);

        TimeSpan elapsed = Stopwatch.GetElapsedTime(startedTimestamp);

        string? correlationId =
            context.Items[CorrelationIdMiddleware.ItemKey]?.ToString();

        LogRequestCompleted(
            logger,
            context.Request.Method,
            context.Request.Path.Value ?? "/",
            context.Response.StatusCode,
            elapsed.TotalMilliseconds,
            correlationId);
    }

    [LoggerMessage(
        EventId = 1000,
        Level = LogLevel.Information,
        Message = "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds:F2} ms. CorrelationId: {CorrelationId}")]
    private static partial void LogRequestCompleted(
        ILogger logger,
        string method,
        string path,
        int statusCode,
        double elapsedMilliseconds,
        string? correlationId);
}
