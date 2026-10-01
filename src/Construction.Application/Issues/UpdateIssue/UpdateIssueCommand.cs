using Construction.Application.Common.Messaging;
using Construction.Domain.Issues;

namespace Construction.Application.Issues.UpdateIssue;

public sealed record UpdateIssueCommand(
    Guid ProjectId,
    Guid IssueId,
    string Title,
    string Description,
    IssueType Type,
    IssueSeverity Severity,
    Guid? LocationId,
    Guid? ActivityId,
    Guid? ResponsibleUserId,
    DateOnly? DueDate) : ICommand<IssueResponse>;
