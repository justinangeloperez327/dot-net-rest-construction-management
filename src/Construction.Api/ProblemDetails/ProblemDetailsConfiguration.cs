using Construction.Api.Middleware;

namespace Construction.Api.ProblemDetails;

public static class ProblemDetailsConfiguration
{
    public static IServiceCollection AddApiProblemDetails(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] =
                    context.HttpContext.TraceIdentifier;

                if (context.HttpContext.Items.TryGetValue(
                    CorrelationIdMiddleware.ItemKey,
                    out object? correlationId))
                {
                    context.ProblemDetails.Extensions["correlationId"] =
                        correlationId?.ToString();
                }
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
