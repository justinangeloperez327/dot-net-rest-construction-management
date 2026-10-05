using Construction.Api.Configuration;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Construction.Api.Extensions;

public static class ObservabilityServiceCollectionExtensions
{
    public static IServiceCollection AddApiObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        ObservabilityOptions options =
            configuration
                .GetSection(ObservabilityOptions.SectionName)
                .Get<ObservabilityOptions>()
            ?? new ObservabilityOptions();

        Validate(options);

        Uri? otlpEndpoint = string.IsNullOrWhiteSpace(options.OtlpEndpoint)
            ? null
            : new Uri(options.OtlpEndpoint, UriKind.Absolute);

        var telemetry = services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService(options.ServiceName));

        telemetry.WithTracing(tracing =>
        {
            tracing
                .SetSampler(
                    new ParentBasedSampler(
                        new TraceIdRatioBasedSampler(
                            options.TraceSamplingRatio)))
                .AddAspNetCoreInstrumentation(instrumentation =>
                {
                    instrumentation.RecordException = true;
                    instrumentation.Filter = context =>
                        !context.Request.Path.StartsWithSegments(
                            "/health");
                })
                .AddHttpClientInstrumentation(instrumentation =>
                {
                    instrumentation.RecordException = true;
                })
                .AddSource("Npgsql");

            if (otlpEndpoint is not null)
            {
                tracing.AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = otlpEndpoint;
                    exporter.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        });

        telemetry.WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation();

            if (otlpEndpoint is not null)
            {
                metrics.AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = otlpEndpoint;
                    exporter.Protocol = OtlpExportProtocol.Grpc;
                });
            }
        });

        return services;
    }

    private static void Validate(ObservabilityOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ServiceName))
        {
            throw new InvalidOperationException(
                "Observability:ServiceName is required.");
        }

        if (options.TraceSamplingRatio is < 0 or > 1)
        {
            throw new InvalidOperationException(
                "Observability:TraceSamplingRatio must be between 0 and 1.");
        }

        if (!string.IsNullOrWhiteSpace(options.OtlpEndpoint)
            && (!Uri.TryCreate(
                    options.OtlpEndpoint,
                    UriKind.Absolute,
                    out Uri? endpoint)
                || (endpoint.Scheme != Uri.UriSchemeHttp
                    && endpoint.Scheme != Uri.UriSchemeHttps)))
        {
            throw new InvalidOperationException(
                "Observability:OtlpEndpoint must be an absolute HTTP or HTTPS URI.");
        }
    }
}
