using Construction.Domain.DailyProgress;

namespace Construction.Application.DailyProgress;

public sealed record DailyProgressReportResponse(
    Guid Id,
    Guid ProjectId,
    DateOnly ReportDate,
    Guid CreatedByUserId,
    WeatherCondition Weather,
    decimal? TemperatureCelsius,
    string? WorkSummary,
    string? Remarks,
    DailyProgressStatus Status,
    Guid? SubmittedByUserId,
    DateTimeOffset? SubmittedAtUtc,
    Guid? ReviewedByUserId,
    DateTimeOffset? ReviewedAtUtc,
    string? RejectionReason,
    IReadOnlyCollection<DailyProgressActivityResponse> Activities,
    IReadOnlyCollection<DailyProgressManpowerResponse> Manpower,
    IReadOnlyCollection<DailyProgressEquipmentResponse> Equipment,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static DailyProgressReportResponse FromDomain(
        DailyProgressReport report)
    {
        ArgumentNullException.ThrowIfNull(report);

        return new DailyProgressReportResponse(
            report.Id,
            report.ProjectId,
            report.ReportDate,
            report.CreatedByUserId,
            report.Weather,
            report.TemperatureCelsius,
            report.WorkSummary,
            report.Remarks,
            report.Status,
            report.SubmittedByUserId,
            report.SubmittedAtUtc,
            report.ReviewedByUserId,
            report.ReviewedAtUtc,
            report.RejectionReason,
            report.Activities
                .Select(DailyProgressActivityResponse.FromDomain)
                .ToArray(),
            report.Manpower
                .Select(DailyProgressManpowerResponse.FromDomain)
                .ToArray(),
            report.Equipment
                .Select(DailyProgressEquipmentResponse.FromDomain)
                .ToArray(),
            report.CreatedAtUtc,
            report.LastModifiedAtUtc);
    }
}

public sealed record DailyProgressActivityResponse(
    Guid Id,
    Guid ActivityId,
    string WorkDescription,
    decimal ReportedProgressPercentage,
    decimal? QuantityCompleted,
    string? Unit)
{
    public static DailyProgressActivityResponse FromDomain(
        DailyProgressActivity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);

        return new DailyProgressActivityResponse(
            activity.Id,
            activity.ActivityId,
            activity.WorkDescription,
            activity.ReportedProgressPercentage,
            activity.QuantityCompleted,
            activity.Unit);
    }
}

public sealed record DailyProgressManpowerResponse(
    Guid Id,
    Guid? CompanyId,
    string Trade,
    int Headcount,
    decimal TotalHours)
{
    public static DailyProgressManpowerResponse FromDomain(
        DailyProgressManpower manpower)
    {
        ArgumentNullException.ThrowIfNull(manpower);

        return new DailyProgressManpowerResponse(
            manpower.Id,
            manpower.CompanyId,
            manpower.Trade,
            manpower.Headcount,
            manpower.TotalHours);
    }
}

public sealed record DailyProgressEquipmentResponse(
    Guid Id,
    string Description,
    int Quantity,
    decimal WorkingHours,
    decimal IdleHours)
{
    public static DailyProgressEquipmentResponse FromDomain(
        DailyProgressEquipment equipment)
    {
        ArgumentNullException.ThrowIfNull(equipment);

        return new DailyProgressEquipmentResponse(
            equipment.Id,
            equipment.Description,
            equipment.Quantity,
            equipment.WorkingHours,
            equipment.IdleHours);
    }
}
