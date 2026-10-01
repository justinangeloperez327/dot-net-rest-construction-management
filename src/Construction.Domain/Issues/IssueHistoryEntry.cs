using Construction.Domain.Common;

namespace Construction.Domain.Issues;

public sealed class IssueHistoryEntry : Entity<Guid>
{
    private IssueHistoryEntry()
        : base(Guid.Empty)
    {
    }

    internal IssueHistoryEntry(
        Guid id,
        Guid issueId,
        IssueHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
        : base(id)
    {
        IssueId = issueId;
        Action = action;
        ActorUserId = actorUserId;
        OccurredAtUtc = occurredAtUtc;
        Note = NormalizeNote(note);
    }

    public Guid IssueId { get; private set; }

    public IssueHistoryAction Action { get; private set; }

    public Guid ActorUserId { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public string? Note { get; private set; }

    private static string? NormalizeNote(string? note)
    {
        if (note?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Issue history note cannot exceed 2000 characters.");
        }

        return string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
