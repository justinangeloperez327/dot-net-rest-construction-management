using Construction.Api.Controllers.V1;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Construction.IntegrationTests.Infrastructure;

public sealed class ConstructionWebApplicationFactory(
    string connectionString,
    string fileStorageRoot)
    : WebApplicationFactory<AuthenticationController>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting(
            "ConnectionStrings:Database",
            connectionString);
        builder.UseSetting(
            "Authentication:Jwt:Issuer",
            "Construction.IntegrationTests");
        builder.UseSetting(
            "Authentication:Jwt:Audience",
            "Construction.IntegrationTests.Client");
        builder.UseSetting(
            "Authentication:Jwt:SigningKey",
            "integration-testing-signing-key-2026-at-least-32-bytes");
        builder.UseSetting(
            "Authentication:Jwt:AccessTokenMinutes",
            "15");
        builder.UseSetting(
            "Authentication:Jwt:RefreshTokenDays",
            "7");
        builder.UseSetting(
            "FileStorage:RootPath",
            fileStorageRoot);
        builder.UseSetting(
            "Api:RateLimitPermitLimit",
            "10000");
    }
}
