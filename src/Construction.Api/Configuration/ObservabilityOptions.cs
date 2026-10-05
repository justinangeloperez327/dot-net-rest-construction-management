namespace Construction.Api.Configuration;

public sealed class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public string ServiceName { get; init; } = "Construction.Api";

    public string? OtlpEndpoint { get; init; }

    public double TraceSamplingRatio { get; init; } = 1.0;
}
