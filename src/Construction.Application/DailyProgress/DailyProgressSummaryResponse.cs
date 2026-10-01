using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress;

public sealed record DailyProgressSummaryResponse(
    Guid Id,
    Guid ProjectId,
    DateOnly ReportDate,
    WeatherCondition Weather,
    DailyProgressStatus Status,
    Guid CreatedByUserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? SubmittedAtUtc,
    DateTimeOffset? ReviewedAtUtc)
{
    public static DailyProgressSummaryResponse FromDomain(
        DailyProgressReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        return new DailyProgressSummaryResponse(
            report.Id,
            report.ProjectId,
            report.ReportDate,
            report.Weather,
            report.Status,
            report.CreatedByUserId,
            report.CreatedAtUtc,
            report.SubmittedAtUtc,
            report.ReviewedAtUtc);
    }
}
