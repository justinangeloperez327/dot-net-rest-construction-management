using Construction.Application.Common.Messaging;
using Construction.Domain.Issues;

namespace Construction.Application.Issues.CreateIssue;

public sealed record CreateIssueCommand(
    Guid ProjectId,
    string Number,
    string Title,
    string Description,
    IssueType Type,
    IssueSeverity Severity,
    Guid? LocationId,
    Guid? ActivityId,
    Guid? ResponsibleUserId,
    DateOnly? DueDate) : ICommand<IssueResponse>;
