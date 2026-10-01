using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.DailyProgress.ReviewDailyProgress;

public sealed class ReviewDailyProgressCommandHandler(
    IDailyProgressRepository reports,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ReviewDailyProgressCommand, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        ReviewDailyProgressCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.DailyProgress.Approve,
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

        if (command.Approve)
        {
            report.Approve(userId, timeProvider.GetUtcNow());
        }
        else
        {
            report.Reject(
                userId,
                timeProvider.GetUtcNow(),
                command.RejectionReason ?? string.Empty);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            DailyProgressReportResponse.FromDomain(report));
    }
}
