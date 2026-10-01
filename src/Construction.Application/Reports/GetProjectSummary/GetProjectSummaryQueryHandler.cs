using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Reports;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Reports.GetProjectSummary;

public sealed class GetProjectSummaryQueryHandler(
    IProjectRepository projects,
    IProjectReportingReadService reporting,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : IQueryHandler<GetProjectSummaryQuery, ProjectSummaryReportResponse>
{
    public async Task<Result<ProjectSummaryReportResponse>> HandleAsync(
        GetProjectSummaryQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            query.ProjectId,
            Permissions.Reports.View,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ProjectSummaryReportResponse>(accessError);
        }

        if (await projects.GetAsync(query.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<ProjectSummaryReportResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        DateOnly today = DateOnly.FromDateTime(
            timeProvider.GetUtcNow().UtcDateTime);

        return Result.Success(
            await reporting.GetSummaryAsync(
                query.ProjectId,
                today,
                cancellationToken));
    }
}
