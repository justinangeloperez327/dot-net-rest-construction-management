using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress.SetDailyProgressActivities;

public sealed class SetDailyProgressActivitiesCommandHandler(
    IDailyProgressRepository reports,
    IActivityRepository activities,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<SetDailyProgressActivitiesCommand, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        SetDailyProgressActivitiesCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

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

        foreach (DailyProgressActivityEntry entry in command.Activities)
        {
            var activity = await activities.GetAsync(
                entry.ActivityId,
                cancellationToken);

            if (activity is null || activity.ProjectId != command.ProjectId)
            {
                return Result.Failure<DailyProgressReportResponse>(
                    ApplicationError.Validation(
                        "DailyProgress.InvalidActivity",
                        $"Activity '{entry.ActivityId}' does not belong to the project."));
            }
        }

        report.ReplaceActivities(
            command.Activities.Select(entry =>
                new DailyProgressActivityInput(
                    entry.ActivityId,
                    entry.WorkDescription,
                    entry.ReportedProgressPercentage,
                    entry.QuantityCompleted,
                    entry.Unit)));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            DailyProgressReportResponse.FromDomain(report));
    }
}
