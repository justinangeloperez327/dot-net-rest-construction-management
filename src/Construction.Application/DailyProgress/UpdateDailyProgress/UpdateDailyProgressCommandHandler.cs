using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.DailyProgress.UpdateDailyProgress;

public sealed class UpdateDailyProgressCommandHandler(
    IDailyProgressRepository reports,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateDailyProgressCommand, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        UpdateDailyProgressCommand command,
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

        report.UpdateHeader(
            command.Weather,
            command.TemperatureCelsius,
            command.WorkSummary,
            command.Remarks);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            DailyProgressReportResponse.FromDomain(report));
    }
}
