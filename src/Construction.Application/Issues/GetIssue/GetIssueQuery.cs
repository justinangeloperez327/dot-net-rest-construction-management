using Construction.Application.Common.Messaging;

namespace Construction.Application.Issues.GetIssue;

public sealed record GetIssueQuery(
    Guid ProjectId,
    Guid IssueId) : IQuery<IssueResponse>;
