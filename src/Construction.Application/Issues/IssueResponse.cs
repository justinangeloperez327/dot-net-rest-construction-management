using Construction.Domain.Issues;

namespace Construction.Application.Issues;

public sealed record IssueResponse(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Title,
    string Description,
    IssueType Type,
    IssueSeverity Severity,
    Guid? LocationId,
    Guid? ActivityId,
    Guid? ResponsibleUserId,
    DateOnly? DueDate,
    Guid CreatedByUserId,
    IssueStatus Status,
    string? ResolutionSummary,
    Guid? VerifiedByUserId,
    DateTimeOffset? VerifiedAtUtc,
    IReadOnlyCollection<CorrectiveActionResponse> CorrectiveActions,
    IReadOnlyCollection<IssueHistoryResponse> History,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static IssueResponse FromDomain(Issue issue)
    {
        ArgumentNullException.ThrowIfNull(issue);

        return new IssueResponse(
            issue.Id,
            issue.ProjectId,
            issue.Number,
            issue.Title,
            issue.Description,
            issue.Type,
            issue.Severity,
            issue.LocationId,
            issue.ActivityId,
            issue.ResponsibleUserId,
            issue.DueDate,
            issue.CreatedByUserId,
            issue.Status,
            issue.ResolutionSummary,
            issue.VerifiedByUserId,
            issue.VerifiedAtUtc,
            issue.CorrectiveActions
                .OrderBy(action => action.CreatedAtUtc)
                .Select(CorrectiveActionResponse.FromDomain)
                .ToArray(),
            issue.History
                .OrderBy(entry => entry.OccurredAtUtc)
                .Select(IssueHistoryResponse.FromDomain)
                .ToArray(),
            issue.CreatedAtUtc,
            issue.LastModifiedAtUtc);
    }
}

public sealed record CorrectiveActionResponse(
    Guid Id,
    string Description,
    Guid? ResponsibleUserId,
    DateOnly? DueDate,
    CorrectiveActionStatus Status,
    string? CompletionNotes,
    Guid? CompletedByUserId,
    DateTimeOffset? CompletedAtUtc,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static CorrectiveActionResponse FromDomain(
        CorrectiveAction action) =>
        new(
            action.Id,
            action.Description,
            action.ResponsibleUserId,
            action.DueDate,
            action.Status,
            action.CompletionNotes,
            action.CompletedByUserId,
            action.CompletedAtUtc,
            action.CreatedAtUtc,
            action.LastModifiedAtUtc);
}

public sealed record IssueHistoryResponse(
    Guid Id,
    IssueHistoryAction Action,
    Guid ActorUserId,
    DateTimeOffset OccurredAtUtc,
    string? Note)
{
    public static IssueHistoryResponse FromDomain(
        IssueHistoryEntry entry) =>
        new(
            entry.Id,
            entry.Action,
            entry.ActorUserId,
            entry.OccurredAtUtc,
            entry.Note);
}
