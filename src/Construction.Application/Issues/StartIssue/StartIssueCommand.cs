using Construction.Application.Common.Messaging;

namespace Construction.Application.Issues.StartIssue;

public sealed record StartIssueCommand(
    Guid ProjectId,
    Guid IssueId) : ICommand<IssueResponse>;
