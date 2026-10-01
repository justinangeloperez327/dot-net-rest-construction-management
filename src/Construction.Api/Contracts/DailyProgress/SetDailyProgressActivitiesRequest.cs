namespace Construction.Api.Contracts.DailyProgress;

public sealed record SetDailyProgressActivitiesRequest(
    IReadOnlyCollection<DailyProgressActivityRequestItem> Activities);

public sealed record DailyProgressActivityRequestItem(
    Guid ActivityId,
    string WorkDescription,
    decimal ReportedProgressPercentage,
    decimal? QuantityCompleted,
    string? Unit);
