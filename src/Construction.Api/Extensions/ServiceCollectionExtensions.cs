using System.Text.Json.Serialization;
using Construction.Api.Authentication;
using Construction.Api.Configuration;
using Construction.Api.ProblemDetails;
using Construction.Application.Abstractions.Authentication;
using Construction.Application.Attachments.DeleteAttachment;
using Construction.Application.Attachments.DownloadAttachment;
using Construction.Application.Attachments.GetAttachments;
using Construction.Application.Attachments.UploadAttachment;
using Construction.Application.Documents.AddDocumentRevision;
using Construction.Application.Documents.ArchiveDocument;
using Construction.Application.Documents.CreateDocument;
using Construction.Application.Documents.DownloadDocumentRevision;
using Construction.Application.Documents.GetDocument;
using Construction.Application.Documents.GetDocuments;
using Construction.Application.Documents.UpdateDocument;
using Construction.Application.Activities.Assignments;
using Construction.Application.Activities.ChangeActivityStatus;
using Construction.Application.Activities.CreateActivity;
using Construction.Application.Activities.Dependencies;
using Construction.Application.Activities.GetActivities;
using Construction.Application.Activities.GetActivity;
using Construction.Application.Activities.UpdateActivity;
using Construction.Application.Activities.UpdateActivityProgress;
using Construction.Application.WorkPackages.ChangeWorkPackageStatus;
using Construction.Application.WorkPackages.CreateWorkPackage;
using Construction.Application.WorkPackages.GetWorkPackage;
using Construction.Application.WorkPackages.GetWorkPackages;
using Construction.Application.WorkPackages.UpdateWorkPackage;
using Construction.Application.Authentication.Login;
using Construction.Application.Authentication.Logout;
using Construction.Application.Authentication.RefreshToken;
using Construction.Application.Companies.CreateCompany;
using Construction.Application.Companies.DeactivateCompany;
using Construction.Application.Companies.GetCompanies;
using Construction.Application.Companies.GetCompany;
using Construction.Application.Companies.UpdateCompany;
using Construction.Application.DailyProgress.CreateDailyProgress;
using Construction.Application.DailyProgress.DeleteDailyProgress;
using Construction.Application.DailyProgress.GetDailyProgress;
using Construction.Application.DailyProgress.GetDailyProgressReports;
using Construction.Application.DailyProgress.ReviewDailyProgress;
using Construction.Application.DailyProgress.SetDailyProgressActivities;
using Construction.Application.DailyProgress.SetDailyProgressEquipment;
using Construction.Application.DailyProgress.SetDailyProgressManpower;
using Construction.Application.DailyProgress.SubmitDailyProgress;
using Construction.Application.DailyProgress.UpdateDailyProgress;
using Construction.Application.Inspections.CancelInspection;
using Construction.Application.Inspections.CreateInspection;
using Construction.Application.Inspections.GetInspection;
using Construction.Application.Inspections.GetInspections;
using Construction.Application.Inspections.PerformInspection;
using Construction.Application.Inspections.RequestInspection;
using Construction.Application.Inspections.UpdateInspection;
using Construction.Application.Issues.CancelIssue;
using Construction.Application.Issues.CorrectiveActions;
using Construction.Application.Issues.CreateIssue;
using Construction.Application.Issues.GetIssue;
using Construction.Application.Issues.GetIssues;
using Construction.Application.Issues.StartIssue;
using Construction.Application.Issues.SubmitIssueForVerification;
using Construction.Application.Issues.UpdateIssue;
using Construction.Application.Issues.VerifyIssue;
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
using Construction.Application.Rfis.AddRfiComment;
using Construction.Application.Rfis.ChangeRfiStatus;
using Construction.Application.Rfis.CreateRfi;
using Construction.Application.Rfis.GetRfi;
using Construction.Application.Rfis.GetRfis;
using Construction.Application.Rfis.OpenRfi;
using Construction.Application.Rfis.RespondRfi;
using Construction.Application.Rfis.UpdateRfi;
using Construction.Application.Submittals.AddSubmittalComment;
using Construction.Application.Submittals.AddSubmittalRevision;
using Construction.Application.Submittals.ChangeSubmittalStatus;
using Construction.Application.Submittals.CreateSubmittal;
using Construction.Application.Submittals.GetSubmittal;
using Construction.Application.Submittals.GetSubmittals;
using Construction.Application.Submittals.ReviewSubmittal;
using Construction.Application.Submittals.StartSubmittalReview;
using Construction.Application.Submittals.SubmitSubmittal;
using Construction.Application.Submittals.UpdateSubmittal;
using Microsoft.AspNetCore.Http.Features;
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

        long maximumFileSizeBytes = long.TryParse(
            configuration["FileStorage:MaximumFileSizeBytes"],
            out long configuredMaximumFileSizeBytes)
                ? configuredMaximumFileSizeBytes
                : 104_857_600;

        if (maximumFileSizeBytes <= 0)
        {
            throw new InvalidOperationException(
                "FileStorage:MaximumFileSizeBytes must be greater than zero.");
        }

        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = maximumFileSizeBytes;
        });

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

        services.AddScoped<CreateWorkPackageCommandHandler>();
        services.AddScoped<GetWorkPackageQueryHandler>();
        services.AddScoped<GetWorkPackagesQueryHandler>();
        services.AddScoped<UpdateWorkPackageCommandHandler>();
        services.AddScoped<ChangeWorkPackageStatusCommandHandler>();

        services.AddScoped<CreateActivityCommandHandler>();
        services.AddScoped<GetActivityQueryHandler>();
        services.AddScoped<GetActivitiesQueryHandler>();
        services.AddScoped<UpdateActivityCommandHandler>();
        services.AddScoped<UpdateActivityProgressCommandHandler>();
        services.AddScoped<ChangeActivityStatusCommandHandler>();
        services.AddScoped<AddActivityAssignmentCommandHandler>();
        services.AddScoped<GetActivityAssignmentsQueryHandler>();
        services.AddScoped<RemoveActivityAssignmentCommandHandler>();
        services.AddScoped<AddActivityDependencyCommandHandler>();
        services.AddScoped<GetActivityDependenciesQueryHandler>();
        services.AddScoped<RemoveActivityDependencyCommandHandler>();

        services.AddScoped<CreateDailyProgressCommandHandler>();
        services.AddScoped<GetDailyProgressQueryHandler>();
        services.AddScoped<GetDailyProgressReportsQueryHandler>();
        services.AddScoped<UpdateDailyProgressCommandHandler>();
        services.AddScoped<SetDailyProgressActivitiesCommandHandler>();
        services.AddScoped<SetDailyProgressManpowerCommandHandler>();
        services.AddScoped<SetDailyProgressEquipmentCommandHandler>();
        services.AddScoped<SubmitDailyProgressCommandHandler>();
        services.AddScoped<ReviewDailyProgressCommandHandler>();
        services.AddScoped<DeleteDailyProgressCommandHandler>();

        services.AddScoped<CreateDocumentCommandHandler>();
        services.AddScoped<GetDocumentQueryHandler>();
        services.AddScoped<GetDocumentsQueryHandler>();
        services.AddScoped<UpdateDocumentCommandHandler>();
        services.AddScoped<ArchiveDocumentCommandHandler>();
        services.AddScoped<AddDocumentRevisionCommandHandler>();
        services.AddScoped<DownloadDocumentRevisionQueryHandler>();

        services.AddScoped<UploadAttachmentCommandHandler>();
        services.AddScoped<GetAttachmentsQueryHandler>();
        services.AddScoped<DownloadAttachmentQueryHandler>();
        services.AddScoped<DeleteAttachmentCommandHandler>();

        services.AddScoped<CreateRfiCommandHandler>();
        services.AddScoped<GetRfiQueryHandler>();
        services.AddScoped<GetRfisQueryHandler>();
        services.AddScoped<UpdateRfiCommandHandler>();
        services.AddScoped<OpenRfiCommandHandler>();
        services.AddScoped<RespondRfiCommandHandler>();
        services.AddScoped<ChangeRfiStatusCommandHandler>();
        services.AddScoped<AddRfiCommentCommandHandler>();

        services.AddScoped<CreateSubmittalCommandHandler>();
        services.AddScoped<GetSubmittalQueryHandler>();
        services.AddScoped<GetSubmittalsQueryHandler>();
        services.AddScoped<UpdateSubmittalCommandHandler>();
        services.AddScoped<AddSubmittalRevisionCommandHandler>();
        services.AddScoped<SubmitSubmittalCommandHandler>();
        services.AddScoped<StartSubmittalReviewCommandHandler>();
        services.AddScoped<ReviewSubmittalCommandHandler>();
        services.AddScoped<ChangeSubmittalStatusCommandHandler>();
        services.AddScoped<AddSubmittalCommentCommandHandler>();

        services.AddScoped<CreateInspectionCommandHandler>();
        services.AddScoped<GetInspectionQueryHandler>();
        services.AddScoped<GetInspectionsQueryHandler>();
        services.AddScoped<UpdateInspectionCommandHandler>();
        services.AddScoped<RequestInspectionCommandHandler>();
        services.AddScoped<StartInspectionCommandHandler>();
        services.AddScoped<CompleteInspectionCommandHandler>();
        services.AddScoped<CancelInspectionCommandHandler>();

        services.AddScoped<CreateIssueCommandHandler>();
        services.AddScoped<GetIssueQueryHandler>();
        services.AddScoped<GetIssuesQueryHandler>();
        services.AddScoped<UpdateIssueCommandHandler>();
        services.AddScoped<StartIssueCommandHandler>();
        services.AddScoped<AddCorrectiveActionCommandHandler>();
        services.AddScoped<UpdateCorrectiveActionCommandHandler>();
        services.AddScoped<SubmitIssueForVerificationCommandHandler>();
        services.AddScoped<VerifyIssueCommandHandler>();
        services.AddScoped<CancelIssueCommandHandler>();

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
