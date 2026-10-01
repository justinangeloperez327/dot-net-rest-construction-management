using Construction.Application.Reports;

namespace Construction.Application.Abstractions.Reports;

public interface IProjectReportingReadService
{
    Task<ProjectSummaryReportResponse> GetSummaryAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default);

    Task<ActivityReportResponse> GetActivitiesAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default);

    Task<DailyProgressReportResponse> GetDailyProgressAsync(
        Guid projectId,
        DateOnly fromDate,
        DateOnly toDate,
        CancellationToken cancellationToken = default);

    Task<QualityReportResponse> GetQualityAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default);

    Task<ProcurementReportResponse> GetProcurementAsync(
        Guid projectId,
        DateOnly today,
        CancellationToken cancellationToken = default);
}
