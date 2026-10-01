using Construction.Domain.Common;

namespace Construction.Domain.Submittals;

public sealed class Submittal : AuditableAggregateRoot<Guid>
{
    private readonly List<SubmittalRevision> _revisions = [];
    private readonly List<SubmittalComment> _comments = [];
    private readonly List<SubmittalHistoryEntry> _history = [];

    private Submittal()
        : base(Guid.Empty)
    {
    }

    private Submittal(
        Guid id,
        Guid projectId,
        string number,
        string title,
        SubmittalType type,
        Guid? responsibleUserId,
        Guid createdByUserId)
        : base(id)
    {
        ProjectId = projectId;
        Number = number;
        NormalizedNumber = NormalizeNumber(number);
        Title = title;
        Type = type;
        ResponsibleUserId = responsibleUserId;
        CreatedByUserId = createdByUserId;
        Status = SubmittalStatus.Draft;
    }

    public Guid ProjectId { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public string NormalizedNumber { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public SubmittalType Type { get; private set; }

    public Guid? ResponsibleUserId { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public SubmittalStatus Status { get; private set; }

    public int CurrentVersionNumber { get; private set; }

    public IReadOnlyCollection<SubmittalRevision> Revisions => _revisions;

    public IReadOnlyCollection<SubmittalComment> Comments => _comments;

    public IReadOnlyCollection<SubmittalHistoryEntry> History => _history;

    public static Submittal Create(
        Guid projectId,
        string number,
        string title,
        SubmittalType type,
        Guid? responsibleUserId,
        Guid createdByUserId,
        DateTimeOffset createdAtUtc)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project identifier is required.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Submittal creator identifier is required.");
        }

        ValidateNumber(number);
        ValidateTitle(title);

        var submittal = new Submittal(
            Guid.CreateVersion7(),
            projectId,
            number.Trim(),
            title.Trim(),
            type,
            responsibleUserId,
            createdByUserId);

        submittal.AddHistory(
            SubmittalHistoryAction.Created,
            createdByUserId,
            createdAtUtc,
            null);

        return submittal;
    }

    public void Update(
        string title,
        SubmittalType type,
        Guid? responsibleUserId,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is not SubmittalStatus.Draft
            and not SubmittalStatus.Rejected)
        {
            throw new DomainException(
                "Only draft or rejected submittals can be updated.");
        }

        ValidateTitle(title);

        Title = title.Trim();
        Type = type;
        ResponsibleUserId = responsibleUserId;

        AddHistory(
            SubmittalHistoryAction.Updated,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public SubmittalRevision AddRevision(
        string revisionCode,
        string? description,
        Guid createdByUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is not SubmittalStatus.Draft
            and not SubmittalStatus.Rejected)
        {
            throw new DomainException(
                "A new revision can only be added to a draft or rejected submittal.");
        }

        if (string.IsNullOrWhiteSpace(revisionCode)
            || revisionCode.Trim().Length > 50)
        {
            throw new DomainException(
                "Revision code is required and cannot exceed 50 characters.");
        }

        if (description?.Trim().Length > 4000)
        {
            throw new DomainException(
                "Revision description cannot exceed 4000 characters.");
        }

        if (_revisions.Any(revision =>
            string.Equals(
                revision.RevisionCode,
                revisionCode.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException(
                "The submittal revision code already exists.");
        }

        foreach (SubmittalRevision revision in _revisions.Where(r => r.IsCurrent))
        {
            revision.Supersede();
        }

        var newRevision = new SubmittalRevision(
            Guid.CreateVersion7(),
            Id,
            CurrentVersionNumber + 1,
            revisionCode.Trim(),
            description,
            createdByUserId);

        _revisions.Add(newRevision);
        CurrentVersionNumber = newRevision.VersionNumber;
        Status = SubmittalStatus.Draft;

        AddHistory(
            SubmittalHistoryAction.RevisionAdded,
            createdByUserId,
            occurredAtUtc,
            revisionCode.Trim());

        return newRevision;
    }

    public void SubmitCurrentRevision(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        DateOnly? reviewDueDate)
    {
        SubmittalRevision revision = GetCurrentRevision();

        revision.Submit(
            actorUserId,
            occurredAtUtc,
            reviewDueDate);

        Status = SubmittalStatus.Submitted;

        AddHistory(
            SubmittalHistoryAction.Submitted,
            actorUserId,
            occurredAtUtc,
            revision.RevisionCode);
    }

    public void StartReview(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        SubmittalRevision revision = GetCurrentRevision();

        revision.StartReview();
        Status = SubmittalStatus.UnderReview;

        AddHistory(
            SubmittalHistoryAction.ReviewStarted,
            actorUserId,
            occurredAtUtc,
            revision.RevisionCode);
    }

    public void ReviewCurrentRevision(
        SubmittalRevisionStatus outcome,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? remarks)
    {
        SubmittalRevision revision = GetCurrentRevision();

        revision.Review(
            outcome,
            actorUserId,
            occurredAtUtc,
            remarks);

        (Status, SubmittalHistoryAction action) = outcome switch
        {
            SubmittalRevisionStatus.Approved =>
                (SubmittalStatus.Approved, SubmittalHistoryAction.Approved),
            SubmittalRevisionStatus.ApprovedWithComments =>
                (SubmittalStatus.ApprovedWithComments,
                    SubmittalHistoryAction.ApprovedWithComments),
            SubmittalRevisionStatus.Rejected =>
                (SubmittalStatus.Rejected, SubmittalHistoryAction.Rejected),
            _ => throw new DomainException(
                "The requested review outcome is invalid.")
        };

        AddHistory(
            action,
            actorUserId,
            occurredAtUtc,
            remarks);
    }

    public void Close(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is not SubmittalStatus.Approved
            and not SubmittalStatus.ApprovedWithComments)
        {
            throw new DomainException(
                "Only approved submittals can be closed.");
        }

        Status = SubmittalStatus.Closed;

        AddHistory(
            SubmittalHistoryAction.Closed,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Cancel(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? reason)
    {
        if (Status is SubmittalStatus.Closed
            or SubmittalStatus.Cancelled)
        {
            throw new DomainException(
                "The submittal cannot be cancelled from its current status.");
        }

        Status = SubmittalStatus.Cancelled;

        AddHistory(
            SubmittalHistoryAction.Cancelled,
            actorUserId,
            occurredAtUtc,
            reason);
    }

    public SubmittalComment AddComment(
        Guid authorUserId,
        string body,
        DateTimeOffset occurredAtUtc)
    {
        if (Status == SubmittalStatus.Cancelled)
        {
            throw new DomainException(
                "Cancelled submittals cannot receive comments.");
        }

        var comment = new SubmittalComment(
            Guid.CreateVersion7(),
            Id,
            authorUserId,
            body);

        _comments.Add(comment);

        AddHistory(
            SubmittalHistoryAction.CommentAdded,
            authorUserId,
            occurredAtUtc,
            null);

        return comment;
    }

    private SubmittalRevision GetCurrentRevision() =>
        _revisions.SingleOrDefault(revision => revision.IsCurrent)
        ?? throw new DomainException(
            "The submittal does not have a current revision.");

    private void AddHistory(
        SubmittalHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
    {
        if (actorUserId == Guid.Empty)
        {
            throw new DomainException(
                "Submittal history actor identifier is required.");
        }

        _history.Add(new SubmittalHistoryEntry(
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
            throw new DomainException("Submittal number is required.");
        }

        if (number.Trim().Length > 100)
        {
            throw new DomainException(
                "Submittal number cannot exceed 100 characters.");
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Submittal title is required.");
        }

        if (title.Trim().Length > 300)
        {
            throw new DomainException(
                "Submittal title cannot exceed 300 characters.");
        }
    }

    private static string NormalizeNumber(string value) =>
        value.Trim().ToUpperInvariant();
}
