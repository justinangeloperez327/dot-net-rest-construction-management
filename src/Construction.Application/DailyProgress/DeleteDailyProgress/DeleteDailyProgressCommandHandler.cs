using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.DailyProgress.DeleteDailyProgress;

public sealed class DeleteDailyProgressCommandHandler(
    IDailyProgressRepository reports,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<DeleteDailyProgressCommand>
{
    public async Task<Result> HandleAsync(
        DeleteDailyProgressCommand command,
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
            return Result.Failure(accessError);
        }

        var report = await reports.GetAsync(
            command.ReportId,
            cancellationToken);

        if (report is null || report.ProjectId != command.ProjectId)
        {
            return Result.Failure(
                ApplicationError.NotFound(
                    "DailyProgress.NotFound",
                    "The daily progress report was not found."));
        }

        if (!report.CanDelete())
        {
            return Result.Failure(
                ApplicationError.Conflict(
                    "DailyProgress.CannotDelete",
                    "Only draft or rejected daily progress reports can be deleted."));
        }

        reports.Remove(report);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
