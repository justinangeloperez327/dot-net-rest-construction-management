using Construction.Domain.DailyProgress;

namespace Construction.Api.Contracts.DailyProgress;

public sealed record UpdateDailyProgressRequest(
    WeatherCondition Weather,
    decimal? TemperatureCelsius,
    string? WorkSummary,
    string? Remarks);
