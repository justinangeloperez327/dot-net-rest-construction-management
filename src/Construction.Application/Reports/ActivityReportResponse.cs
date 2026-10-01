using Construction.Domain.Activities;

namespace Construction.Application.Reports;

public sealed record ActivityReportResponse(
    Guid ProjectId,
    decimal AverageProgressPercentage,
    ActivitySummaryCounts Counts,
    IReadOnlyCollection<OverdueActivityReportItem> OverdueActivities);

public sealed record OverdueActivityReportItem(
    Guid Id,
    string Code,
    string Name,
    ActivityStatus Status,
    decimal ProgressPercentage,
    DateOnly PlannedEndDate,
    Guid? WorkPackageId,
    Guid? LocationId);
