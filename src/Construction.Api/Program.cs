using System.Diagnostics;
using Construction.Api.Configuration;
using Construction.Api.Extensions;
using Construction.Api.Hosting;
using Construction.Infrastructure;
using Construction.Infrastructure.Authentication;
using Construction.Infrastructure.Files;
using Construction.Infrastructure.Persistence;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

if (ContainerHealthProbe.IsRequested(args))
{
    Environment.ExitCode =
        await ContainerHealthProbe.RunAsync(args);

    return;
}

bool migrateOnly = args.Contains(
    "--migrate",
    StringComparer.Ordinal);

string[] hostArguments = args
    .Where(argument =>
        !string.Equals(
            argument,
            "--migrate",
            StringComparison.Ordinal))
    .ToArray();

var builder = WebApplication.CreateBuilder(hostArguments);

ProductionConfigurationValidator.Validate(
    builder.Configuration,
    builder.Environment);

if (builder.Environment.IsProduction())
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole(options =>
    {
        options.IncludeScopes = true;
        options.UseUtcTimestamp = true;
        options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
    });
}

string databaseConnectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "Connection string 'Database' is required.");

var jwtOptions = new JwtOptions
{
    Issuer = builder.Configuration["Authentication:Jwt:Issuer"] ?? string.Empty,
    Audience = builder.Configuration["Authentication:Jwt:Audience"] ?? string.Empty,
    SigningKey = builder.Configuration["Authentication:Jwt:SigningKey"] ?? string.Empty,
    AccessTokenMinutes = int.TryParse(
        builder.Configuration["Authentication:Jwt:AccessTokenMinutes"],
        out int accessTokenMinutes)
            ? accessTokenMinutes
            : 15,
    RefreshTokenDays = int.TryParse(
        builder.Configuration["Authentication:Jwt:RefreshTokenDays"],
        out int refreshTokenDays)
            ? refreshTokenDays
            : 7
};

string configuredFileStoragePath =
    builder.Configuration["FileStorage:RootPath"]
    ?? "data/uploads";

string fileStoragePath = Path.IsPathRooted(configuredFileStoragePath)
    ? configuredFileStoragePath
    : Path.Combine(
        builder.Environment.ContentRootPath,
        configuredFileStoragePath);

var fileStorageOptions = new FileStorageOptions
{
    RootPath = fileStoragePath,
    MaximumFileSizeBytes = long.TryParse(
        builder.Configuration["FileStorage:MaximumFileSizeBytes"],
        out long maximumFileSizeBytes)
            ? maximumFileSizeBytes
            : 104_857_600
};

builder.Services.Configure<HostOptions>(options =>
{
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddApiObservability(builder.Configuration);
builder.Services.AddInfrastructure(
    databaseConnectionString,
    jwtOptions,
    fileStorageOptions);

var app = builder.Build();

if (migrateOnly)
{
    await using AsyncServiceScope scope =
        app.Services.CreateAsyncScope();

    DatabaseInitializer initializer =
        scope.ServiceProvider
            .GetRequiredService<DatabaseInitializer>();

    await initializer.InitializeAsync();

    return;
}

app.UseApiPipeline();
app.MapApiEndpoints();

await app.RunAsync();

public partial class Program;
