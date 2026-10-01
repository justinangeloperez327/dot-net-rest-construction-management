using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Reports;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Reports.GetQualityReport;

public sealed class GetQualityReportQueryHandler(
    IProjectRepository projects,
    IProjectReportingReadService reporting,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : IQueryHandler<GetQualityReportQuery, QualityReportResponse>
{
    public async Task<Result<QualityReportResponse>> HandleAsync(
        GetQualityReportQuery query,
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
            return Result.Failure<QualityReportResponse>(accessError);
        }

        if (await projects.GetAsync(query.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<QualityReportResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        DateOnly today = DateOnly.FromDateTime(
            timeProvider.GetUtcNow().UtcDateTime);

        return Result.Success(
            await reporting.GetQualityAsync(
                query.ProjectId,
                today,
                cancellationToken));
    }
}
