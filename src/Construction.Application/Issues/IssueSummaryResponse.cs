using Construction.Domain.Issues;

namespace Construction.Application.Issues;

public sealed record IssueSummaryResponse(
    Guid Id,
    string Number,
    string Title,
    IssueType Type,
    IssueSeverity Severity,
    IssueStatus Status,
    Guid? LocationId,
    Guid? ResponsibleUserId,
    DateOnly? DueDate,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static IssueSummaryResponse FromDomain(Issue issue) =>
        new(
            issue.Id,
            issue.Number,
            issue.Title,
            issue.Type,
            issue.Severity,
            issue.Status,
            issue.LocationId,
            issue.ResponsibleUserId,
            issue.DueDate,
            issue.CreatedAtUtc,
            issue.LastModifiedAtUtc);
}
