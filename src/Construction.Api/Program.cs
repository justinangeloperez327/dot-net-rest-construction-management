using Construction.Api.Extensions;
using Construction.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

string databaseConnectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "Connection string 'Database' is required.");

builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddInfrastructure(databaseConnectionString);

var app = builder.Build();

app.UseApiPipeline();
app.MapApiEndpoints();

app.Run();

public partial class Program;
