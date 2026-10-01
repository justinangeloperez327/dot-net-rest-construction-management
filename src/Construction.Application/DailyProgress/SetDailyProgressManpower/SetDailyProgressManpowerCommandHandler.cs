using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Companies;
using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress.SetDailyProgressManpower;

public sealed class SetDailyProgressManpowerCommandHandler(
    IDailyProgressRepository reports,
    ICompanyRepository companies,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<SetDailyProgressManpowerCommand, DailyProgressReportResponse>
{
    public async Task<Result<DailyProgressReportResponse>> HandleAsync(
        SetDailyProgressManpowerCommand command,
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

        foreach (DailyProgressManpowerEntry entry in command.Manpower)
        {
            if (entry.CompanyId is not Guid companyId)
            {
                continue;
            }

            Company? company = await companies.GetAsync(
                companyId,
                cancellationToken);

            if (company is null || company.Status != CompanyStatus.Active)
            {
                return Result.Failure<DailyProgressReportResponse>(
                    ApplicationError.Validation(
                        "DailyProgress.InvalidCompany",
                        $"Company '{companyId}' is not an active company."));
            }
        }

        report.ReplaceManpower(
            command.Manpower.Select(entry =>
                new DailyProgressManpowerInput(
                    entry.CompanyId,
                    entry.Trade,
                    entry.Headcount,
                    entry.TotalHours)));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            DailyProgressReportResponse.FromDomain(report));
    }
}
