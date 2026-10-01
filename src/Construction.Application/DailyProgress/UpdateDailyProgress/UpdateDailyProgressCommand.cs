using Construction.Application.Common.Messaging;
using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress.UpdateDailyProgress;

public sealed record UpdateDailyProgressCommand(
    Guid ProjectId,
    Guid ReportId,
    WeatherCondition Weather,
    decimal? TemperatureCelsius,
    string? WorkSummary,
    string? Remarks) : ICommand<DailyProgressReportResponse>;
