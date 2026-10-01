using Construction.Domain.Issues;

namespace Construction.Api.Contracts.Issues;

public sealed record UpdateIssueRequest(
    string Title,
    string Description,
    IssueType Type,
    IssueSeverity Severity,
    Guid? LocationId,
    Guid? ActivityId,
    Guid? ResponsibleUserId,
    DateOnly? DueDate);
