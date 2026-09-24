using Construction.Api.Configuration;
using Construction.Api.HealthChecks;
using Construction.Api.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Construction.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseExceptionHandler();
        app.UseHttpsRedirection();
        app.UseCors(ApiCorsPolicy.Name);
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.MapControllers();

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = _ => false,
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = registration =>
                registration.Tags.Contains("ready"),
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi("/openapi/{documentName}.json");
        }

        return app;
    }
}
