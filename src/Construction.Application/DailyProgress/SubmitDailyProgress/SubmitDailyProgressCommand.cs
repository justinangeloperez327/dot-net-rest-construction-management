using Construction.Application.Common.Messaging;

namespace Construction.Application.DailyProgress.SubmitDailyProgress;

public sealed record SubmitDailyProgressCommand(
    Guid ProjectId,
    Guid ReportId) : ICommand<DailyProgressReportResponse>;
