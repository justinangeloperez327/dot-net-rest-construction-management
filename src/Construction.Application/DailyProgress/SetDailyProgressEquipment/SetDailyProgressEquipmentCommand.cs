using Construction.Application.Common.Messaging;

namespace Construction.Application.DailyProgress.SetDailyProgressEquipment;

public sealed record SetDailyProgressEquipmentCommand(
    Guid ProjectId,
    Guid ReportId,
    IReadOnlyCollection<DailyProgressEquipmentEntry> Equipment)
    : ICommand<DailyProgressReportResponse>;

public sealed record DailyProgressEquipmentEntry(
    string Description,
    int Quantity,
    decimal WorkingHours,
    decimal IdleHours);
