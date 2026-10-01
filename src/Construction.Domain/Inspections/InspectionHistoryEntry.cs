using Construction.Domain.Common;

namespace Construction.Domain.Inspections;

public sealed class InspectionHistoryEntry : Entity<Guid>
{
    private InspectionHistoryEntry()
        : base(Guid.Empty)
    {
    }

    internal InspectionHistoryEntry(
        Guid id,
        Guid inspectionId,
        InspectionHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
        : base(id)
    {
        InspectionId = inspectionId;
        Action = action;
        ActorUserId = actorUserId;
        OccurredAtUtc = occurredAtUtc;
        Note = NormalizeNote(note);
    }

    public Guid InspectionId { get; private set; }

    public InspectionHistoryAction Action { get; private set; }

    public Guid ActorUserId { get; private set; }

    public DateTimeOffset OccurredAtUtc { get; private set; }

    public string? Note { get; private set; }

    private static string? NormalizeNote(string? note)
    {
        if (note?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Inspection history note cannot exceed 2000 characters.");
        }

        return string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}
