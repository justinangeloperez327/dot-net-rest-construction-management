using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress.SetDailyProgressEquipment;

public sealed class SetDailyProgressEquipmentCommandHandler(
    IDailyProgressRepository reports,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<SetDailyProgressEquipmentCommand, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        SetDailyProgressEquipmentCommand command,
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

        report.ReplaceEquipment(
            command.Equipment.Select(entry =>
                new DailyProgressEquipmentInput(
                    entry.Description,
                    entry.Quantity,
                    entry.WorkingHours,
                    entry.IdleHours)));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            DailyProgressReportResponse.FromDomain(report));
    }
}
