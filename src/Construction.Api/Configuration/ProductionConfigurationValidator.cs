namespace Construction.Api.Configuration;

public static class ProductionConfigurationValidator
{
    public static void Validate(
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(environment);

        if (!environment.IsProduction())
        {
            return;
        }

        string? signingKey =
            configuration["Authentication:Jwt:SigningKey"];

        if (string.IsNullOrWhiteSpace(signingKey)
            || signingKey.Contains(
                "development-only",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Production requires a non-development JWT signing key supplied through secure configuration.");
        }

        string[] allowedOrigins =
            configuration
                .GetSection("Api:AllowedOrigins")
                .Get<string[]>()
            ?? [];

        foreach (string origin in allowedOrigins)
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out Uri? uri)
                || uri.Scheme != Uri.UriSchemeHttps)
            {
                throw new InvalidOperationException(
                    "Production API origins must be absolute HTTPS URIs.");
            }
        }
    }
}
