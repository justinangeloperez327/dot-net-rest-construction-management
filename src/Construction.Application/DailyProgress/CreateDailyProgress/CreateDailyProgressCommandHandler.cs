using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress.CreateDailyProgress;

public sealed class CreateDailyProgressCommandHandler(
    IProjectRepository projects,
    IDailyProgressRepository reports,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<CreateDailyProgressCommand, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        CreateDailyProgressCommand command,
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

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        DateOnly today =
            DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        if (command.ReportDate > today)
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.Validation(
                    "DailyProgress.FutureDate",
                    "A daily progress report cannot be created for a future date."));
        }

        if (await reports.ExistsForDateAsync(
            command.ProjectId,
            command.ReportDate,
            cancellationToken: cancellationToken))
        {
            return Result.Failure<DailyProgressReportResponse>(
                ApplicationError.Conflict(
                    "DailyProgress.DateAlreadyExists",
                    "A daily progress report already exists for this project and date."));
        }

        DailyProgressReport report = DailyProgressReport.Create(
            command.ProjectId,
            command.ReportDate,
            userId,
            command.Weather,
            command.TemperatureCelsius,
            command.WorkSummary,
            command.Remarks);

        reports.Add(report);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            DailyProgressReportResponse.FromDomain(report));
    }
}
