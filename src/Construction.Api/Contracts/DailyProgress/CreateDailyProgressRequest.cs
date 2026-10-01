using Construction.Domain.DailyProgress;

namespace Construction.Api.Contracts.DailyProgress;

public sealed record CreateDailyProgressRequest(
    DateOnly ReportDate,
    WeatherCondition Weather,
    decimal? TemperatureCelsius,
    string? WorkSummary,
    string? Remarks);
