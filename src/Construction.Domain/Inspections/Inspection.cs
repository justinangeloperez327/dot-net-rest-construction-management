using Construction.Domain.Common;

namespace Construction.Domain.Inspections;

public sealed class Inspection : AuditableAggregateRoot<Guid>
{
    private readonly List<InspectionHistoryEntry> _history = [];

    private Inspection()
        : base(Guid.Empty)
    {
    }

    private Inspection(
        Guid id,
        Guid projectId,
        string number,
        string title,
        string? description,
        InspectionType type,
        Guid? locationId,
        Guid? activityId,
        DateOnly? requestedForDate,
        Guid? inspectorUserId,
        Guid requestedByUserId)
        : base(id)
    {
        ProjectId = projectId;
        Number = number;
        NormalizedNumber = NormalizeNumber(number);
        Title = title;
        Description = NormalizeOptional(description);
        Type = type;
        LocationId = locationId;
        ActivityId = activityId;
        RequestedForDate = requestedForDate;
        InspectorUserId = inspectorUserId;
        RequestedByUserId = requestedByUserId;
        Status = InspectionStatus.Draft;
    }

    public Guid ProjectId { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public string NormalizedNumber { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public InspectionType Type { get; private set; }

    public Guid? LocationId { get; private set; }

    public Guid? ActivityId { get; private set; }

    public DateOnly? RequestedForDate { get; private set; }

    public Guid? InspectorUserId { get; private set; }

    public Guid RequestedByUserId { get; private set; }

    public InspectionStatus Status { get; private set; }

    public string? ResultNotes { get; private set; }

    public Guid? InspectedByUserId { get; private set; }

    public DateTimeOffset? InspectedAtUtc { get; private set; }

    public IReadOnlyCollection<InspectionHistoryEntry> History => _history;

    public static Inspection Create(
        Guid projectId,
        string number,
        string title,
        string? description,
        InspectionType type,
        Guid? locationId,
        Guid? activityId,
        DateOnly? requestedForDate,
        Guid? inspectorUserId,
        Guid requestedByUserId,
        DateTimeOffset createdAtUtc)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project identifier is required.");
        }

        if (requestedByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Inspection requester identifier is required.");
        }

        ValidateNumber(number);
        ValidateDetails(title, description);

        var inspection = new Inspection(
            Guid.CreateVersion7(),
            projectId,
            number.Trim(),
            title.Trim(),
            description,
            type,
            locationId,
            activityId,
            requestedForDate,
            inspectorUserId,
            requestedByUserId);

        inspection.AddHistory(
            InspectionHistoryAction.Created,
            requestedByUserId,
            createdAtUtc,
            null);

        return inspection;
    }

    public void Update(
        string title,
        string? description,
        InspectionType type,
        Guid? locationId,
        Guid? activityId,
        DateOnly? requestedForDate,
        Guid? inspectorUserId,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is not InspectionStatus.Draft
            and not InspectionStatus.Requested)
        {
            throw new DomainException(
                "Only draft or requested inspections can be updated.");
        }

        ValidateDetails(title, description);

        Title = title.Trim();
        Description = NormalizeOptional(description);
        Type = type;
        LocationId = locationId;
        ActivityId = activityId;
        RequestedForDate = requestedForDate;
        InspectorUserId = inspectorUserId;

        AddHistory(
            InspectionHistoryAction.Updated,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Request(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != InspectionStatus.Draft)
        {
            throw new DomainException(
                "Only draft inspections can be requested.");
        }

        Status = InspectionStatus.Requested;

        AddHistory(
            InspectionHistoryAction.Requested,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Start(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != InspectionStatus.Requested)
        {
            throw new DomainException(
                "Only requested inspections can be started.");
        }

        Status = InspectionStatus.InProgress;

        AddHistory(
            InspectionHistoryAction.Started,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Complete(
        bool passed,
        string resultNotes,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != InspectionStatus.InProgress)
        {
            throw new DomainException(
                "Only in-progress inspections can be completed.");
        }

        if (string.IsNullOrWhiteSpace(resultNotes))
        {
            throw new DomainException(
                "Inspection result notes are required.");
        }

        if (resultNotes.Trim().Length > 5000)
        {
            throw new DomainException(
                "Inspection result notes cannot exceed 5000 characters.");
        }

        Status = passed
            ? InspectionStatus.Passed
            : InspectionStatus.Failed;
        ResultNotes = resultNotes.Trim();
        InspectedByUserId = actorUserId;
        InspectedAtUtc = occurredAtUtc;

        AddHistory(
            passed
                ? InspectionHistoryAction.Passed
                : InspectionHistoryAction.Failed,
            actorUserId,
            occurredAtUtc,
            resultNotes);
    }

    public void Cancel(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? reason)
    {
        if (Status is InspectionStatus.Passed
            or InspectionStatus.Failed
            or InspectionStatus.Cancelled)
        {
            throw new DomainException(
                "The inspection cannot be cancelled from its current status.");
        }

        Status = InspectionStatus.Cancelled;

        AddHistory(
            InspectionHistoryAction.Cancelled,
            actorUserId,
            occurredAtUtc,
            reason);
    }

    private void AddHistory(
        InspectionHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
    {
        if (actorUserId == Guid.Empty)
        {
            throw new DomainException(
                "Inspection history actor identifier is required.");
        }

        _history.Add(new InspectionHistoryEntry(
            Guid.CreateVersion7(),
            Id,
            action,
            actorUserId,
            occurredAtUtc,
            note));
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("Inspection number is required.");
        }

        if (number.Trim().Length > 100)
        {
            throw new DomainException(
                "Inspection number cannot exceed 100 characters.");
        }
    }

    private static void ValidateDetails(
        string title,
        string? description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Inspection title is required.");
        }

        if (title.Trim().Length > 300)
        {
            throw new DomainException(
                "Inspection title cannot exceed 300 characters.");
        }

        if (description?.Trim().Length > 5000)
        {
            throw new DomainException(
                "Inspection description cannot exceed 5000 characters.");
        }
    }

    private static string NormalizeNumber(string value) =>
        value.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
