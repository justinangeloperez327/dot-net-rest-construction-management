using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Reports;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Reports.GetDailyProgressReport;

public sealed class GetDailyProgressReportQueryHandler(
    IProjectRepository projects,
    IProjectReportingReadService reporting,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : IQueryHandler<GetDailyProgressReportQuery, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        GetDailyProgressReportQuery query,
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
            return Result.Failure<DailyProgressReportResponse>(accessError);
        }

        if (await projects.GetAsync(query.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        DateOnly today = DateOnly.FromDateTime(
            timeProvider.GetUtcNow().UtcDateTime);

        DateOnly toDate = query.ToDate ?? today;
        DateOnly fromDate = query.FromDate ?? toDate.AddDays(-29);

        if (fromDate > toDate)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.Validation(
                    "Reports.InvalidDateRange",
                    "Report start date cannot be after the end date."));
        }

        if (toDate.DayNumber - fromDate.DayNumber > 366)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.Validation(
                    "Reports.DateRangeTooLarge",
                    "Daily progress reporting is limited to 367 calendar days per request."));
        }

        return Result.Success(
            await reporting.GetDailyProgressAsync(
                query.ProjectId,
                fromDate,
                toDate,
                cancellationToken));
    }
}
