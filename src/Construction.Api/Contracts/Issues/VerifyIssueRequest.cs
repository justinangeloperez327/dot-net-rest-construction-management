namespace Construction.Api.Contracts.Issues;

public sealed record VerifyIssueRequest(
    bool Close,
    string? ReopenReason);
