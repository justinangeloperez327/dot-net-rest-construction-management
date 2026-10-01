using Construction.Application.Common.Messaging;

namespace Construction.Application.Issues.CancelIssue;

public sealed record CancelIssueCommand(
    Guid ProjectId,
    Guid IssueId,
    string? Reason) : ICommand<IssueResponse>;
