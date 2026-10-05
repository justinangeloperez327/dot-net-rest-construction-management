using System.Diagnostics;

namespace Construction.Api.Middleware;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";
    public const string TraceHeaderName = "X-Trace-ID";
    public const string ItemKey = "CorrelationId";

    private const int MaximumCorrelationIdLength = 64;

    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        string correlationId = GetCorrelationId(context);
        string? traceId = Activity.Current?.TraceId.ToString();

        context.Items[ItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        if (!string.IsNullOrWhiteSpace(traceId))
        {
            context.Response.Headers[TraceHeaderName] = traceId;
        }

        Activity.Current?.SetTag(
            "app.correlation_id",
            correlationId);

        await next(context);
    }

    private static string GetCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(HeaderName, out var values))
        {
            string? supplied = values.FirstOrDefault();

            if (IsValid(supplied))
            {
                return supplied!;
            }
        }

        return Guid.CreateVersion7().ToString();
    }

    private static bool IsValid(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Length > MaximumCorrelationIdLength)
        {
            return false;
        }

        return value.All(character =>
            char.IsAsciiLetterOrDigit(character)
            || character is '-' or '_' or '.');
    }
}
