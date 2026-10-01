using Construction.Application.Common.Messaging;

namespace Construction.Application.Reports.GetDailyProgressReport;

public sealed record GetDailyProgressReportQuery(
    Guid ProjectId,
    DateOnly? FromDate,
    DateOnly? ToDate) : IQuery<DailyProgressReportResponse>;
