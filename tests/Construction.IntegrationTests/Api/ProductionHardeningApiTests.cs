using System.Net;
using Construction.Api.Middleware;
using Construction.IntegrationTests.Infrastructure;
using Xunit;

namespace Construction.IntegrationTests.Api;

[Collection(IntegrationTestGroup.Name)]
public sealed class ProductionHardeningApiTests(
    IntegrationTestFixture fixture)
{
    [Fact]
    public async Task LiveHealth_ReturnsSecurityAndCorrelationHeaders()
    {
        using HttpClient client = fixture.CreateClient();

        const string correlationId = "integration-test-123";
        client.DefaultRequestHeaders.Add(
            CorrelationIdMiddleware.HeaderName,
            correlationId);

        HttpResponseMessage response =
            await client.GetAsync(
                "/health/live",
                TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.True(
            response.Headers.TryGetValues(
                CorrelationIdMiddleware.HeaderName,
                out IEnumerable<string>? correlationValues));

        Assert.Equal(
            correlationId,
            Assert.Single(correlationValues));

        Assert.Equal(
            "nosniff",
            Assert.Single(
                response.Headers.GetValues(
                    "X-Content-Type-Options")));

        Assert.Equal(
            "DENY",
            Assert.Single(
                response.Headers.GetValues(
                    "X-Frame-Options")));

        Assert.Equal(
            "no-referrer",
            Assert.Single(
                response.Headers.GetValues(
                    "Referrer-Policy")));

        Assert.True(
            response.Headers.Contains(
                CorrelationIdMiddleware.TraceHeaderName));
    }

    [Fact]
    public async Task InvalidCorrelationId_IsNotReflected()
    {
        using HttpClient client = fixture.CreateClient();

        const string invalidCorrelationId = "invalid correlation value";

        client.DefaultRequestHeaders.Add(
            CorrelationIdMiddleware.HeaderName,
            invalidCorrelationId);

        HttpResponseMessage response =
            await client.GetAsync(
                "/health/live",
                TestContext.Current.CancellationToken);

        Assert.True(
            response.Headers.TryGetValues(
                CorrelationIdMiddleware.HeaderName,
                out IEnumerable<string>? values));

        string returned = Assert.Single(values);

        Assert.NotEqual(
            invalidCorrelationId,
            returned);
        Assert.DoesNotContain(' ', returned);
    }

    [Fact]
    public async Task ReadyHealth_ReportsDatabaseReady()
    {
        using HttpClient client = fixture.CreateClient();

        HttpResponseMessage response =
            await client.GetAsync(
                "/health/ready",
                TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RateLimitExceeded_ReturnsProblemDetailsAndRetryAfter()
    {
        using var factory =
            new ConstructionWebApplicationFactory(
                fixture.ConnectionString,
                fixture.FileStorageRoot,
                rateLimitPermitLimit: 1,
                rateLimitWindowSeconds: 60);

        using HttpClient client =
            factory.CreateClient(
                new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
                {
                    BaseAddress = new Uri("https://localhost"),
                    AllowAutoRedirect = false
                });

        CancellationToken cancellationToken =
            TestContext.Current.CancellationToken;

        HttpResponseMessage first =
            await client.GetAsync(
                "/health/live",
                cancellationToken);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        HttpResponseMessage second =
            await client.GetAsync(
                "/health/live",
                cancellationToken);

        Assert.Equal(
            HttpStatusCode.TooManyRequests,
            second.StatusCode);

        Assert.True(
            second.Headers.Contains("Retry-After"));

        Assert.Equal(
            "application/problem+json",
            second.Content.Headers.ContentType?.MediaType);
    }
}
