using Construction.Domain.Rfis;

namespace Construction.Application.Rfis;

public sealed record RfiResponse(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Subject,
    string Question,
    DateOnly? DueDate,
    Guid? ResponsibleUserId,
    Guid RaisedByUserId,
    RfiStatus Status,
    string? Response,
    Guid? RespondedByUserId,
    DateTimeOffset? RespondedAtUtc,
    Guid? ClosedByUserId,
    DateTimeOffset? ClosedAtUtc,
    IReadOnlyCollection<RfiCommentResponse> Comments,
    IReadOnlyCollection<RfiHistoryResponse> History,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static RfiResponse FromDomain(Rfi rfi)
    {
        ArgumentNullException.ThrowIfNull(rfi);

        return new RfiResponse(
            rfi.Id,
            rfi.ProjectId,
            rfi.Number,
            rfi.Subject,
            rfi.Question,
            rfi.DueDate,
            rfi.ResponsibleUserId,
            rfi.RaisedByUserId,
            rfi.Status,
            rfi.Response,
            rfi.RespondedByUserId,
            rfi.RespondedAtUtc,
            rfi.ClosedByUserId,
            rfi.ClosedAtUtc,
            rfi.Comments
                .OrderBy(comment => comment.CreatedAtUtc)
                .Select(RfiCommentResponse.FromDomain)
                .ToArray(),
            rfi.History
                .OrderBy(entry => entry.OccurredAtUtc)
                .Select(RfiHistoryResponse.FromDomain)
                .ToArray(),
            rfi.CreatedAtUtc,
            rfi.LastModifiedAtUtc);
    }
}

public sealed record RfiCommentResponse(
    Guid Id,
    Guid AuthorUserId,
    string Body,
    DateTimeOffset CreatedAtUtc)
{
    public static RfiCommentResponse FromDomain(RfiComment comment) =>
        new(
            comment.Id,
            comment.AuthorUserId,
            comment.Body,
            comment.CreatedAtUtc);
}

public sealed record RfiHistoryResponse(
    Guid Id,
    RfiHistoryAction Action,
    Guid ActorUserId,
    DateTimeOffset OccurredAtUtc,
    string? Note)
{
    public static RfiHistoryResponse FromDomain(RfiHistoryEntry entry) =>
        new(
            entry.Id,
            entry.Action,
            entry.ActorUserId,
            entry.OccurredAtUtc,
            entry.Note);
}
