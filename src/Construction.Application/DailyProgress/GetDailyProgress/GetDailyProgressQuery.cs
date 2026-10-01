using Construction.Application.Common.Messaging;

namespace Construction.Application.DailyProgress.GetDailyProgress;

public sealed record GetDailyProgressQuery(
    Guid ProjectId,
    Guid ReportId) : IQuery<DailyProgressReportResponse>;
