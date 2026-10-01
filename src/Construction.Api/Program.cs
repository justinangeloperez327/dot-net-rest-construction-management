using Construction.Api.Extensions;
using Construction.Infrastructure;
using Construction.Infrastructure.Authentication;
using Construction.Infrastructure.Files;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructure(
    databaseConnectionString,
    jwtOptions,
    fileStorageOptions);

var app = builder.Build();

app.UseApiPipeline();
app.MapApiEndpoints();

app.Run();

public partial class Program;
