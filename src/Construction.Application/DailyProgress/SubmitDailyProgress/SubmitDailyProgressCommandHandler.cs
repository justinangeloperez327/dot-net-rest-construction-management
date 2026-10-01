using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.DailyProgress.SubmitDailyProgress;

public sealed class SubmitDailyProgressCommandHandler(
    IDailyProgressRepository reports,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<SubmitDailyProgressCommand, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        SubmitDailyProgressCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.DailyProgress.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<DailyProgressReportResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var report = await reports.GetAsync(
            command.ReportId,
            cancellationToken);

        if (report is null || report.ProjectId != command.ProjectId)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.NotFound(
                    "DailyProgress.NotFound",
                    "The daily progress report was not found."));
        }

        report.Submit(userId, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            DailyProgressReportResponse.FromDomain(report));
    }
}
