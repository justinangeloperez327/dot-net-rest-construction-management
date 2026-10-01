using Construction.Application.Common.Messaging;

namespace Construction.Application.DailyProgress.SetDailyProgressManpower;

public sealed record SetDailyProgressManpowerCommand(
    Guid ProjectId,
    Guid ReportId,
    IReadOnlyCollection<DailyProgressManpowerEntry> Manpower)
    : ICommand<DailyProgressReportResponse>;

public sealed record DailyProgressManpowerEntry(
    Guid? CompanyId,
    string Trade,
    int Headcount,
    decimal TotalHours);
