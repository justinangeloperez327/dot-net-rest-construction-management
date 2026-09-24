namespace Construction.Api.Configuration;

public sealed class ApiOptions
{
    public const string SectionName = "Api";

    public string[] AllowedOrigins { get; init; } = [];

    public int RateLimitPermitLimit { get; init; } = 120;

    public int RateLimitWindowSeconds { get; init; } = 60;

    public int RateLimitQueueLimit { get; init; }
}
