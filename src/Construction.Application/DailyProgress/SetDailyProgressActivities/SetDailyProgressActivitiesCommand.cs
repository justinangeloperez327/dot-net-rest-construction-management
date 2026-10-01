using Construction.Application.Common.Messaging;

namespace Construction.Application.DailyProgress.SetDailyProgressActivities;

public sealed record SetDailyProgressActivitiesCommand(
    Guid ProjectId,
    Guid ReportId,
    IReadOnlyCollection<DailyProgressActivityEntry> Activities)
    : ICommand<DailyProgressReportResponse>;

public sealed record DailyProgressActivityEntry(
    Guid ActivityId,
    string WorkDescription,
    decimal ReportedProgressPercentage,
    decimal? QuantityCompleted,
    string? Unit);
