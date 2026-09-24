using Construction.Api.Configuration;
using Construction.Api.ProblemDetails;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Construction.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        ApiOptions apiOptions =
            configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>()
            ?? new ApiOptions();

        ValidateApiOptions(apiOptions);

        services.AddControllers();
        services.AddApiProblemDetails();
        services.AddOpenApi("v1");

        services.AddCors(options =>
        {
            options.AddPolicy(ApiCorsPolicy.Name, policy =>
            {
                if (apiOptions.AllowedOrigins.Length == 0)
                {
                    return;
                }

                policy
                    .WithOrigins(apiOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                httpContext =>
                {
                    string partitionKey =
                        httpContext.Connection.RemoteIpAddress?.ToString()
                        ?? "unknown";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = apiOptions.RateLimitPermitLimit,
                            Window = TimeSpan.FromSeconds(
                                apiOptions.RateLimitWindowSeconds),
                            QueueLimit = apiOptions.RateLimitQueueLimit,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                });
        });

        return services;
    }

    private static void ValidateApiOptions(ApiOptions options)
    {
        if (options.RateLimitPermitLimit <= 0)
        {
            throw new InvalidOperationException(
                "Api:RateLimitPermitLimit must be greater than zero.");
        }

        if (options.RateLimitWindowSeconds <= 0)
        {
            throw new InvalidOperationException(
                "Api:RateLimitWindowSeconds must be greater than zero.");
        }

        if (options.RateLimitQueueLimit < 0)
        {
            throw new InvalidOperationException(
                "Api:RateLimitQueueLimit cannot be negative.");
        }
    }
}
