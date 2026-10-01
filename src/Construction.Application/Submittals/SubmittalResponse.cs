using Construction.Domain.Submittals;

namespace Construction.Application.Submittals;

public sealed record SubmittalResponse(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Title,
    SubmittalType Type,
    Guid? ResponsibleUserId,
    Guid CreatedByUserId,
    SubmittalStatus Status,
    int CurrentVersionNumber,
    IReadOnlyCollection<SubmittalRevisionResponse> Revisions,
    IReadOnlyCollection<SubmittalCommentResponse> Comments,
    IReadOnlyCollection<SubmittalHistoryResponse> History,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static SubmittalResponse FromDomain(Submittal submittal)
    {
        ArgumentNullException.ThrowIfNull(submittal);

        return new SubmittalResponse(
            submittal.Id,
            submittal.ProjectId,
            submittal.Number,
            submittal.Title,
            submittal.Type,
            submittal.ResponsibleUserId,
            submittal.CreatedByUserId,
            submittal.Status,
            submittal.CurrentVersionNumber,
            submittal.Revisions
                .OrderByDescending(revision => revision.VersionNumber)
                .Select(SubmittalRevisionResponse.FromDomain)
                .ToArray(),
            submittal.Comments
                .OrderBy(comment => comment.CreatedAtUtc)
                .Select(SubmittalCommentResponse.FromDomain)
                .ToArray(),
            submittal.History
                .OrderBy(entry => entry.OccurredAtUtc)
                .Select(SubmittalHistoryResponse.FromDomain)
                .ToArray(),
            submittal.CreatedAtUtc,
            submittal.LastModifiedAtUtc);
    }
}

public sealed record SubmittalRevisionResponse(
    Guid Id,
    int VersionNumber,
    string RevisionCode,
    string? Description,
    Guid CreatedByUserId,
    SubmittalRevisionStatus Status,
    bool IsCurrent,
    Guid? SubmittedByUserId,
    DateTimeOffset? SubmittedAtUtc,
    DateOnly? ReviewDueDate,
    Guid? ReviewedByUserId,
    DateTimeOffset? ReviewedAtUtc,
    string? ReviewRemarks)
{
    public static SubmittalRevisionResponse FromDomain(SubmittalRevision revision) =>
        new(
            revision.Id,
            revision.VersionNumber,
            revision.RevisionCode,
            revision.Description,
            revision.CreatedByUserId,
            revision.Status,
            revision.IsCurrent,
            revision.SubmittedByUserId,
            revision.SubmittedAtUtc,
            revision.ReviewDueDate,
            revision.ReviewedByUserId,
            revision.ReviewedAtUtc,
            revision.ReviewRemarks);
}

public sealed record SubmittalCommentResponse(
    Guid Id,
    Guid AuthorUserId,
    string Body,
    DateTimeOffset CreatedAtUtc)
{
    public static SubmittalCommentResponse FromDomain(SubmittalComment comment) =>
        new(
            comment.Id,
            comment.AuthorUserId,
            comment.Body,
            comment.CreatedAtUtc);
}

public sealed record SubmittalHistoryResponse(
    Guid Id,
    SubmittalHistoryAction Action,
    Guid ActorUserId,
    DateTimeOffset OccurredAtUtc,
    string? Note)
{
    public static SubmittalHistoryResponse FromDomain(SubmittalHistoryEntry entry) =>
        new(
            entry.Id,
            entry.Action,
            entry.ActorUserId,
            entry.OccurredAtUtc,
            entry.Note);
}
