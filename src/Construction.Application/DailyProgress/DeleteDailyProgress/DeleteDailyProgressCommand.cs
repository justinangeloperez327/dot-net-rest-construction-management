using Construction.Application.Common.Messaging;

namespace Construction.Application.DailyProgress.DeleteDailyProgress;

public sealed record DeleteDailyProgressCommand(
    Guid ProjectId,
    Guid ReportId) : ICommand;
