using Construction.Application.Common.Messaging;
using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress.CreateDailyProgress;

public sealed record CreateDailyProgressCommand(
    Guid ProjectId,
    DateOnly ReportDate,
    WeatherCondition Weather,
    decimal? TemperatureCelsius,
    string? WorkSummary,
    string? Remarks) : ICommand<DailyProgressReportResponse>;
