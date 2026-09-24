using Construction.Api.Extensions;
using Construction.Infrastructure;
using Construction.Infrastructure.Authentication;

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

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructure(
    databaseConnectionString,
    jwtOptions);

var app = builder.Build();

app.UseApiPipeline();
app.MapApiEndpoints();

app.Run();

public partial class Program;
