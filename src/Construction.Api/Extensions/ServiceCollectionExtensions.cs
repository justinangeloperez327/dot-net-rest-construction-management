using System.Text.Json.Serialization;
using Construction.Api.Authentication;
using Construction.Api.Configuration;
using Construction.Api.ProblemDetails;
using Construction.Application.Abstractions.Authentication;
using Construction.Application.Authentication.Login;
using Construction.Application.Authentication.Logout;
using Construction.Application.Authentication.RefreshToken;
using Construction.Application.Companies.CreateCompany;
using Construction.Application.Companies.DeactivateCompany;
using Construction.Application.Companies.GetCompanies;
using Construction.Application.Companies.GetCompany;
using Construction.Application.Companies.UpdateCompany;
using Construction.Application.Locations.CreateLocation;
using Construction.Application.Locations.GetLocations;
using Construction.Application.Locations.UpdateLocation;
using Construction.Application.ProjectMembers.AddProjectMember;
using Construction.Application.ProjectMembers.GetProjectMembers;
using Construction.Application.ProjectMembers.UpdateProjectMember;
using Construction.Application.Projects.ChangeProjectStatus;
using Construction.Application.Projects.CreateProject;
using Construction.Application.Projects.GetProject;
using Construction.Application.Projects.GetProjects;
using Construction.Application.Projects.UpdateProject;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

namespace Construction.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        ApiOptions apiOptions =
            configuration.GetSection(ApiOptions.SectionName).Get<ApiOptions>()
            ?? new ApiOptions();

        ValidateApiOptions(apiOptions);

        services
            .AddControllers()
            .AddJsonOptions(options =>
                options.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter()));

        services.AddApiProblemDetails();
        services.AddOpenApi("v1");
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<RefreshTokenCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();

        services.AddScoped<CreateCompanyCommandHandler>();
        services.AddScoped<GetCompanyQueryHandler>();
        services.AddScoped<GetCompaniesQueryHandler>();
        services.AddScoped<UpdateCompanyCommandHandler>();
        services.AddScoped<DeactivateCompanyCommandHandler>();

        services.AddScoped<CreateProjectCommandHandler>();
        services.AddScoped<GetProjectQueryHandler>();
        services.AddScoped<GetProjectsQueryHandler>();
        services.AddScoped<UpdateProjectCommandHandler>();
        services.AddScoped<ChangeProjectStatusCommandHandler>();

        services.AddScoped<AddProjectMemberCommandHandler>();
        services.AddScoped<GetProjectMembersQueryHandler>();
        services.AddScoped<UpdateProjectMemberCommandHandler>();

        services.AddScoped<CreateLocationCommandHandler>();
        services.AddScoped<GetLocationsQueryHandler>();
        services.AddScoped<UpdateLocationCommandHandler>();

        services.AddCors(options =>
        {
            options.AddPolicy(ApiCorsPolicy.Name, policy =>
            {
                if (apiOptions.AllowedOrigins.Length == 0)
                {
                    return;
                }

                policy
                    .WithOrigins(apiOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(
                    httpContext =>
                    {
                        string partitionKey =
                            httpContext.Connection.RemoteIpAddress?.ToString()
                            ?? "unknown";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = apiOptions.RateLimitPermitLimit,
                                Window = TimeSpan.FromSeconds(
                                    apiOptions.RateLimitWindowSeconds),
                                QueueLimit = apiOptions.RateLimitQueueLimit,
                                QueueProcessingOrder =
                                    QueueProcessingOrder.OldestFirst,
                                AutoReplenishment = true
                            });
                    });
        });

        return services;
    }

    private static void ValidateApiOptions(ApiOptions options)
    {
        if (options.RateLimitPermitLimit <= 0)
        {
            throw new InvalidOperationException(
                "Api:RateLimitPermitLimit must be greater than zero.");
        }

        if (options.RateLimitWindowSeconds <= 0)
        {
            throw new InvalidOperationException(
                "Api:RateLimitWindowSeconds must be greater than zero.");
        }

        if (options.RateLimitQueueLimit < 0)
        {
            throw new InvalidOperationException(
                "Api:RateLimitQueueLimit cannot be negative.");
        }
    }
}
