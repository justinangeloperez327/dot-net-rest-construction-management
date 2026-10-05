namespace Construction.Api.Hosting;

public static class ContainerHealthProbe
{
    public const string SwitchName = "--healthcheck";

    private static readonly HttpClient Client = new()
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    public static bool IsRequested(
        IReadOnlyList<string> arguments) =>
        arguments.Count > 0
        && string.Equals(
            arguments[0],
            SwitchName,
            StringComparison.Ordinal);

    public static async Task<int> RunAsync(
        IReadOnlyList<string> arguments)
    {
        string endpoint = arguments.Count > 1
            ? arguments[1]
            : "http://127.0.0.1:8080/health/live";

        if (!Uri.TryCreate(
                endpoint,
                UriKind.Absolute,
                out Uri? uri)
            || (uri.Scheme != Uri.UriSchemeHttp
                && uri.Scheme != Uri.UriSchemeHttps))
        {
            return 1;
        }

        try
        {
            using HttpResponseMessage response =
                await Client.GetAsync(uri);

            return response.IsSuccessStatusCode
                ? 0
                : 1;
        }
        catch (HttpRequestException)
        {
            return 1;
        }
        catch (TaskCanceledException)
        {
            return 1;
        }
    }
}
