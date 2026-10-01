using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Construction.IntegrationTests.Infrastructure;
using Xunit;

namespace Construction.IntegrationTests.Api;

[Collection(IntegrationTestGroup.Name)]
public sealed class AuthenticationApiTests(
    IntegrationTestFixture fixture)
{
    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsProblemDetails()
    {
        using HttpClient client = fixture.CreateClient();

        CancellationToken cancellationToken =
            TestContext.Current.CancellationToken;

        HttpResponseMessage response =
            await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email = fixture.UserEmail,
                    password = "wrong-password"
                },
                cancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);

        using JsonDocument document =
            JsonDocument.Parse(
                await response.Content.ReadAsStringAsync(
                    cancellationToken));

        Assert.True(
            document.RootElement.TryGetProperty(
                "errors",
                out JsonElement errors));
        Assert.Equal(
            JsonValueKind.Array,
            errors.ValueKind);
    }

    [Fact]
    public async Task Login_ThenMe_ReturnsAuthenticatedUser()
    {
        using HttpClient client = fixture.CreateClient();

        CancellationToken cancellationToken =
            TestContext.Current.CancellationToken;

        string accessToken =
            await LoginAsync(
                client,
                cancellationToken);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        HttpResponseMessage response =
            await client.GetAsync(
                "/api/v1/auth/me",
                cancellationToken);

        Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
    }

    [Fact]
    public async Task ProtectedReport_WithoutToken_ReturnsUnauthorized()
    {
        using HttpClient client = fixture.CreateClient();

        HttpResponseMessage response =
            await client.GetAsync(
                $"/api/v1/projects/{Guid.NewGuid()}/reports/summary",
                TestContext.Current.CancellationToken);

        Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
    }

    [Fact]
    public async Task ProtectedReport_WithoutPermission_ReturnsForbidden()
    {
        using HttpClient client = fixture.CreateClient();

        CancellationToken cancellationToken =
            TestContext.Current.CancellationToken;

        string accessToken =
            await LoginAsync(
                client,
                cancellationToken);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        HttpResponseMessage response =
            await client.GetAsync(
                $"/api/v1/projects/{Guid.NewGuid()}/reports/summary",
                cancellationToken);

        Assert.Equal(
            HttpStatusCode.Forbidden,
            response.StatusCode);
    }

    private async Task<string> LoginAsync(
        HttpClient client,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage response =
            await client.PostAsJsonAsync(
                "/api/v1/auth/login",
                new
                {
                    email = fixture.UserEmail,
                    password = fixture.UserPassword
                },
                cancellationToken);

        response.EnsureSuccessStatusCode();

        using JsonDocument document =
            JsonDocument.Parse(
                await response.Content.ReadAsStringAsync(
                    cancellationToken));

        return document.RootElement
            .GetProperty("accessToken")
            .GetString()
            ?? throw new InvalidOperationException(
                "Access token was not returned.");
    }
}
