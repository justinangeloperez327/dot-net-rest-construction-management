using Construction.Domain.Common;

namespace Construction.Domain.Submittals;

public sealed class SubmittalHistoryEntry : Entity<Guid>
{
    private SubmittalHistoryEntry()
        : base(Guid.Empty)
    {
    }

    internal SubmittalHistoryEntry(
        Guid id,
        Guid submittalId,
        SubmittalHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
        : base(id)
    {
        SubmittalId = submittalId;
        Action = action;
        ActorUserId = actorUserId;
        OccurredAtUtc = occurredAtUtc;
        Note = NormalizeNote(note);
    }

    public Guid SubmittalId { get; private set; }

    public SubmittalHistoryAction Action { get; private set; }

    public Guid ActorUserId { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public string? Note { get; private set; }

    private static string? NormalizeNote(string? note)
    {
        if (note?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Submittal history note cannot exceed 2000 characters.");
        }

        return string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
