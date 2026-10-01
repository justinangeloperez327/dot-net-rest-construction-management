using Construction.Domain.Common;

namespace Construction.Domain.Rfis;

public sealed class Rfi : AuditableAggregateRoot<Guid>
{
    private readonly List<RfiComment> _comments = [];
    private readonly List<RfiHistoryEntry> _history = [];

    private Rfi()
        : base(Guid.Empty)
    {
    }

    private Rfi(
        Guid id,
        Guid projectId,
        string number,
        string subject,
        string question,
        DateOnly? dueDate,
        Guid? responsibleUserId,
        Guid raisedByUserId)
        : base(id)
    {
        ProjectId = projectId;
        Number = number;
        NormalizedNumber = NormalizeNumber(number);
        Subject = subject;
        Question = question;
        DueDate = dueDate;
        ResponsibleUserId = responsibleUserId;
        RaisedByUserId = raisedByUserId;
        Status = RfiStatus.Draft;
    }

    public Guid ProjectId { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public string NormalizedNumber { get; private set; } = string.Empty;

    public string Subject { get; private set; } = string.Empty;

    public string Question { get; private set; } = string.Empty;

    public DateOnly? DueDate { get; private set; }

    public Guid? ResponsibleUserId { get; private set; }

    public Guid RaisedByUserId { get; private set; }

    public RfiStatus Status { get; private set; }

    public string? Response { get; private set; }

    public Guid? RespondedByUserId { get; private set; }

    public DateTimeOffset? RespondedAtUtc { get; private set; }

    public Guid? ClosedByUserId { get; private set; }

    public DateTimeOffset? ClosedAtUtc { get; private set; }

    public IReadOnlyCollection<RfiComment> Comments => _comments;

    public IReadOnlyCollection<RfiHistoryEntry> History => _history;

    public static Rfi Create(
        Guid projectId,
        string number,
        string subject,
        string question,
        DateOnly? dueDate,
        Guid? responsibleUserId,
        Guid raisedByUserId,
        DateTimeOffset createdAtUtc)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project identifier is required.");
        }

        if (raisedByUserId == Guid.Empty)
        {
            throw new DomainException("RFI creator identifier is required.");
        }

        ValidateNumber(number);
        ValidateDetails(subject, question);

        var rfi = new Rfi(
            Guid.CreateVersion7(),
            projectId,
            number.Trim(),
            subject.Trim(),
            question.Trim(),
            dueDate,
            responsibleUserId,
            raisedByUserId);

        rfi.AddHistory(
            RfiHistoryAction.Created,
            raisedByUserId,
            createdAtUtc,
            null);

        return rfi;
    }

    public void Update(
        string subject,
        string question,
        DateOnly? dueDate,
        Guid? responsibleUserId,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is not RfiStatus.Draft and not RfiStatus.Open)
        {
            throw new DomainException(
                "Only draft or open RFIs can be updated.");
        }

        ValidateDetails(subject, question);

        Subject = subject.Trim();
        Question = question.Trim();
        DueDate = dueDate;
        ResponsibleUserId = responsibleUserId;

        AddHistory(
            RfiHistoryAction.Updated,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Open(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != RfiStatus.Draft)
        {
            throw new DomainException(
                "Only draft RFIs can be opened.");
        }

        Status = RfiStatus.Open;

        AddHistory(
            RfiHistoryAction.Opened,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Respond(
        string response,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != RfiStatus.Open)
        {
            throw new DomainException(
                "Only open RFIs can be answered.");
        }

        if (string.IsNullOrWhiteSpace(response))
        {
            throw new DomainException("RFI response is required.");
        }

        if (response.Trim().Length > 10000)
        {
            throw new DomainException(
                "RFI response cannot exceed 10000 characters.");
        }

        Response = response.Trim();
        RespondedByUserId = actorUserId;
        RespondedAtUtc = occurredAtUtc;
        Status = RfiStatus.Answered;

        AddHistory(
            RfiHistoryAction.Responded,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Reopen(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? reason)
    {
        if (Status != RfiStatus.Answered)
        {
            throw new DomainException(
                "Only answered RFIs can be reopened.");
        }

        Status = RfiStatus.Open;
        Response = null;
        RespondedByUserId = null;
        RespondedAtUtc = null;

        AddHistory(
            RfiHistoryAction.Reopened,
            actorUserId,
            occurredAtUtc,
            reason);
    }

    public void Close(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != RfiStatus.Answered)
        {
            throw new DomainException(
                "Only answered RFIs can be closed.");
        }

        Status = RfiStatus.Closed;
        ClosedByUserId = actorUserId;
        ClosedAtUtc = occurredAtUtc;

        AddHistory(
            RfiHistoryAction.Closed,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Cancel(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? reason)
    {
        if (Status is RfiStatus.Closed or RfiStatus.Cancelled)
        {
            throw new DomainException(
                "The RFI cannot be cancelled from its current status.");
        }

        Status = RfiStatus.Cancelled;

        AddHistory(
            RfiHistoryAction.Cancelled,
            actorUserId,
            occurredAtUtc,
            reason);
    }

    public RfiComment AddComment(
        Guid authorUserId,
        string body,
        DateTimeOffset occurredAtUtc)
    {
        if (Status == RfiStatus.Cancelled)
        {
            throw new DomainException(
                "Cancelled RFIs cannot receive comments.");
        }

        var comment = new RfiComment(
            Guid.CreateVersion7(),
            Id,
            authorUserId,
            body);

        _comments.Add(comment);

        AddHistory(
            RfiHistoryAction.CommentAdded,
            authorUserId,
            occurredAtUtc,
            null);

        return comment;
    }

    private void AddHistory(
        RfiHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
    {
        if (actorUserId == Guid.Empty)
        {
            throw new DomainException(
                "RFI history actor identifier is required.");
        }

        _history.Add(new RfiHistoryEntry(
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
            throw new DomainException("RFI number is required.");
        }

        if (number.Trim().Length > 100)
        {
            throw new DomainException(
                "RFI number cannot exceed 100 characters.");
        }
    }

    private static void ValidateDetails(
        string subject,
        string question)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new DomainException("RFI subject is required.");
        }

        if (subject.Trim().Length > 300)
        {
            throw new DomainException(
                "RFI subject cannot exceed 300 characters.");
        }

        if (string.IsNullOrWhiteSpace(question))
        {
            throw new DomainException("RFI question is required.");
        }

        if (question.Trim().Length > 10000)
        {
            throw new DomainException(
                "RFI question cannot exceed 10000 characters.");
        }
    }

    private static string NormalizeNumber(string value) =>
        value.Trim().ToUpperInvariant();
}
