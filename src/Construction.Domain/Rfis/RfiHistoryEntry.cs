using Construction.Domain.Common;

namespace Construction.Domain.Rfis;

public sealed class RfiHistoryEntry : Entity<Guid>
{
    private RfiHistoryEntry()
        : base(Guid.Empty)
    {
    }

    internal RfiHistoryEntry(
        Guid id,
        Guid rfiId,
        RfiHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
        : base(id)
    {
        RfiId = rfiId;
        Action = action;
        ActorUserId = actorUserId;
        OccurredAtUtc = occurredAtUtc;
        Note = NormalizeNote(note);
    }

    public Guid RfiId { get; private set; }

    public RfiHistoryAction Action { get; private set; }

    public Guid ActorUserId { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public string? Note { get; private set; }

    private static string? NormalizeNote(string? note)
    {
        if (note?.Trim().Length > 2000)
        {
            throw new DomainException(
                "RFI history note cannot exceed 2000 characters.");
        }

        return string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
