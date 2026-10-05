using Construction.Api.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace Construction.IntegrationTests.Api;

public sealed class ProductionConfigurationValidatorTests
{
    [Fact]
    public void Validate_RejectsDevelopmentSigningKeyInProduction()
    {
        var configuration = new ConfigurationManager();
        configuration["Authentication:Jwt:SigningKey"] =
            "development-only-signing-key-change-before-use-2026";

        var environment = new TestHostEnvironment(
            Environments.Production);

        Assert.Throws<InvalidOperationException>(
            () => ProductionConfigurationValidator.Validate(
                configuration,
                environment));
    }

    [Fact]
    public void Validate_RejectsHttpCorsOriginInProduction()
    {
        var values = new Dictionary<string, string?>
        {
            ["Authentication:Jwt:SigningKey"] =
                "production-test-signing-key-that-is-at-least-32-bytes",
            ["Api:AllowedOrigins:0"] =
                "http://example.test"
        };

        IConfiguration configuration =
            new ConfigurationBuilder()
                .AddInMemoryCollection(values)
                .Build();

        var environment = new TestHostEnvironment(
            Environments.Production);

        Assert.Throws<InvalidOperationException>(
            () => ProductionConfigurationValidator.Validate(
                configuration,
                environment));
    }

    [Fact]
    public void Validate_AllowsDevelopmentConfigurationOutsideProduction()
    {
        var configuration = new ConfigurationManager();
        configuration["Authentication:Jwt:SigningKey"] =
            "development-only-signing-key-change-before-use-2026";
        configuration["Api:AllowedOrigins:0"] =
            "http://localhost:3000";

        var environment = new TestHostEnvironment(
            Environments.Development);

        ProductionConfigurationValidator.Validate(
            configuration,
            environment);
    }

    private sealed class TestHostEnvironment(
        string environmentName)
        : IHostEnvironment
    {
        public string EnvironmentName { get; set; } =
            environmentName;

        public string ApplicationName { get; set; } =
            "Construction.IntegrationTests";

        public string ContentRootPath { get; set; } =
            Path.GetTempPath();

        public IFileProvider ContentRootFileProvider { get; set; } =
            new NullFileProvider();
    }
}
