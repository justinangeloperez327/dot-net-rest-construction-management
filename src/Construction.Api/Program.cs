using Construction.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

string databaseConnectionString =
    builder.Configuration.GetConnectionString("Database")
    ?? throw new InvalidOperationException(
        "Connection string 'Database' is required.");

builder.Services.AddControllers();
builder.Services.AddInfrastructure(databaseConnectionString);

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
