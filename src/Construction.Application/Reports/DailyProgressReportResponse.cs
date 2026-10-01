namespace Construction.Application.Reports;

public sealed record DailyProgressReportResponse(
    Guid ProjectId,
    DateOnly FromDate,
    DateOnly ToDate,
    DailyProgressSummaryCounts Reports,
    int TotalReportedHeadcount,
    decimal TotalManpowerHours,
    int TotalReportedEquipmentQuantity,
    decimal TotalEquipmentWorkingHours,
    decimal TotalEquipmentIdleHours);
