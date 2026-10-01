namespace Construction.Api.Contracts.DailyProgress;

public sealed record ReviewDailyProgressRequest(
    bool Approve,
    string? RejectionReason);
