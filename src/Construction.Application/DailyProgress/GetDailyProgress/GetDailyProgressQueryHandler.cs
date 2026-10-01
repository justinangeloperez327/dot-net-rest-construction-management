using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.DailyProgress.GetDailyProgress;

public sealed class GetDailyProgressQueryHandler(
    IDailyProgressRepository reports,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetDailyProgressQuery, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        GetDailyProgressQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.DailyProgress.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<DailyProgressReportResponse>(accessError);
        }

        var report = await reports.GetAsync(
            query.ReportId,
            cancellationToken);

        return report is null || report.ProjectId != query.ProjectId
            ? Result.Failure<DailyProgressReportResponse>(
                ApplicationError.NotFound(
                    "DailyProgress.NotFound",
                    "The daily progress report was not found."))
            : Result.Success(
                DailyProgressReportResponse.FromDomain(report));
    }
}
