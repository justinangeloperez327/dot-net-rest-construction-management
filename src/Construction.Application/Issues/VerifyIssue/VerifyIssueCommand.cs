using Construction.Application.Common.Messaging;

namespace Construction.Application.Issues.VerifyIssue;

public sealed record VerifyIssueCommand(
    Guid ProjectId,
    Guid IssueId,
    bool Close,
    string? ReopenReason) : ICommand<IssueResponse>;
