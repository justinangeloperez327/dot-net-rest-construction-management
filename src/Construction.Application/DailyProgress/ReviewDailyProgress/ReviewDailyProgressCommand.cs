using Construction.Application.Common.Messaging;

namespace Construction.Application.DailyProgress.ReviewDailyProgress;

public sealed record ReviewDailyProgressCommand(
    Guid ProjectId,
    Guid ReportId,
    bool Approve,
    string? RejectionReason) : ICommand<DailyProgressReportResponse>;
