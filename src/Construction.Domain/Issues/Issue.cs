using Construction.Domain.Common;

namespace Construction.Domain.Issues;

public sealed class Issue : AuditableAggregateRoot<Guid>
{
    private readonly List<CorrectiveAction> _correctiveActions = [];
    private readonly List<IssueHistoryEntry> _history = [];

    private Issue()
        : base(Guid.Empty)
    {
    }

    private Issue(
        Guid id,
        Guid projectId,
        string number,
        string title,
        string description,
        IssueType type,
        IssueSeverity severity,
        Guid? locationId,
        Guid? activityId,
        Guid? responsibleUserId,
        DateOnly? dueDate,
        Guid createdByUserId)
        : base(id)
    {
        ProjectId = projectId;
        Number = number;
        NormalizedNumber = NormalizeNumber(number);
        Title = title;
        Description = description;
        Type = type;
        Severity = severity;
        LocationId = locationId;
        ActivityId = activityId;
        ResponsibleUserId = responsibleUserId;
        DueDate = dueDate;
        CreatedByUserId = createdByUserId;
        Status = IssueStatus.Open;
    }

    public Guid ProjectId { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public string NormalizedNumber { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public IssueType Type { get; private set; }

    public IssueSeverity Severity { get; private set; }

    public Guid? LocationId { get; private set; }

    public Guid? ActivityId { get; private set; }

    public Guid? ResponsibleUserId { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public IssueStatus Status { get; private set; }

    public string? ResolutionSummary { get; private set; }

    public Guid? VerifiedByUserId { get; private set; }

    public DateTimeOffset? VerifiedAtUtc { get; private set; }

    public IReadOnlyCollection<CorrectiveAction> CorrectiveActions =>
        _correctiveActions;

    public IReadOnlyCollection<IssueHistoryEntry> History => _history;

    public static Issue Create(
        Guid projectId,
        string number,
        string title,
        string description,
        IssueType type,
        IssueSeverity severity,
        Guid? locationId,
        Guid? activityId,
        Guid? responsibleUserId,
        DateOnly? dueDate,
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
                "Issue creator identifier is required.");
        }

        ValidateNumber(number);
        ValidateDetails(title, description);

        var issue = new Issue(
            Guid.CreateVersion7(),
            projectId,
            number.Trim(),
            title.Trim(),
            description.Trim(),
            type,
            severity,
            locationId,
            activityId,
            responsibleUserId,
            dueDate,
            createdByUserId);

        issue.AddHistory(
            IssueHistoryAction.Created,
            createdByUserId,
            createdAtUtc,
            null);

        return issue;
    }

    public void Update(
        string title,
        string description,
        IssueType type,
        IssueSeverity severity,
        Guid? locationId,
        Guid? activityId,
        Guid? responsibleUserId,
        DateOnly? dueDate,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is IssueStatus.PendingVerification
            or IssueStatus.Closed
            or IssueStatus.Cancelled)
        {
            throw new DomainException(
                "Issues pending verification, closed, or cancelled cannot be updated.");
        }

        ValidateDetails(title, description);

        Title = title.Trim();
        Description = description.Trim();
        Type = type;
        Severity = severity;
        LocationId = locationId;
        ActivityId = activityId;
        ResponsibleUserId = responsibleUserId;
        DueDate = dueDate;

        AddHistory(
            IssueHistoryAction.Updated,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Start(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != IssueStatus.Open)
        {
            throw new DomainException(
                "Only open issues can be started.");
        }

        Status = IssueStatus.InProgress;

        AddHistory(
            IssueHistoryAction.Started,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public CorrectiveAction AddCorrectiveAction(
        string description,
        Guid? responsibleUserId,
        DateOnly? dueDate,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is IssueStatus.PendingVerification
            or IssueStatus.Closed
            or IssueStatus.Cancelled)
        {
            throw new DomainException(
                "Corrective actions cannot be added in the current issue status.");
        }

        var action = new CorrectiveAction(
            Guid.CreateVersion7(),
            Id,
            description,
            responsibleUserId,
            dueDate);

        _correctiveActions.Add(action);

        AddHistory(
            IssueHistoryAction.CorrectiveActionAdded,
            actorUserId,
            occurredAtUtc,
            null);

        return action;
    }

    public void UpdateCorrectiveAction(
        Guid correctiveActionId,
        string description,
        Guid? responsibleUserId,
        DateOnly? dueDate,
        CorrectiveActionStatus requestedStatus,
        string? completionNotes,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is IssueStatus.PendingVerification
            or IssueStatus.Closed
            or IssueStatus.Cancelled)
        {
            throw new DomainException(
                "Corrective actions cannot be changed in the current issue status.");
        }

        CorrectiveAction action =
            _correctiveActions.SingleOrDefault(item =>
                item.Id == correctiveActionId)
            ?? throw new DomainException(
                "Corrective action was not found.");

        CorrectiveActionStatus currentStatus = action.Status;

        action.Update(
            description,
            responsibleUserId,
            dueDate);

        if (requestedStatus != currentStatus)
        {
            switch (requestedStatus)
            {
                case CorrectiveActionStatus.InProgress
                    when currentStatus == CorrectiveActionStatus.Pending:
                    action.Start();
                    break;

                case CorrectiveActionStatus.Completed
                    when currentStatus is CorrectiveActionStatus.Pending
                        or CorrectiveActionStatus.InProgress:
                    action.Complete(
                        completionNotes ?? string.Empty,
                        actorUserId,
                        occurredAtUtc);
                    break;

                case CorrectiveActionStatus.Cancelled
                    when currentStatus is CorrectiveActionStatus.Pending
                        or CorrectiveActionStatus.InProgress:
                    action.Cancel();
                    break;

                default:
                    throw new DomainException(
                        "The requested corrective action status transition is invalid.");
            }
        }

        AddHistory(
            IssueHistoryAction.CorrectiveActionUpdated,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void SubmitForVerification(
        string resolutionSummary,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status is not IssueStatus.Open
            and not IssueStatus.InProgress)
        {
            throw new DomainException(
                "Only open or in-progress issues can be submitted for verification.");
        }

        if (_correctiveActions.Any(action =>
            action.Status is CorrectiveActionStatus.Pending
                or CorrectiveActionStatus.InProgress))
        {
            throw new DomainException(
                "All corrective actions must be completed or cancelled before verification.");
        }

        if (string.IsNullOrWhiteSpace(resolutionSummary))
        {
            throw new DomainException(
                "Resolution summary is required.");
        }

        if (resolutionSummary.Trim().Length > 5000)
        {
            throw new DomainException(
                "Resolution summary cannot exceed 5000 characters.");
        }

        ResolutionSummary = resolutionSummary.Trim();
        Status = IssueStatus.PendingVerification;

        AddHistory(
            IssueHistoryAction.SubmittedForVerification,
            actorUserId,
            occurredAtUtc,
            resolutionSummary);
    }

    public void VerifyAndClose(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc)
    {
        if (Status != IssueStatus.PendingVerification)
        {
            throw new DomainException(
                "Only issues pending verification can be closed.");
        }

        Status = IssueStatus.Closed;
        VerifiedByUserId = actorUserId;
        VerifiedAtUtc = occurredAtUtc;

        AddHistory(
            IssueHistoryAction.VerifiedClosed,
            actorUserId,
            occurredAtUtc,
            null);
    }

    public void Reopen(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string reason)
    {
        if (Status != IssueStatus.PendingVerification)
        {
            throw new DomainException(
                "Only issues pending verification can be reopened.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new DomainException(
                "A reopen reason is required.");
        }

        Status = IssueStatus.InProgress;
        ResolutionSummary = null;

        AddHistory(
            IssueHistoryAction.Reopened,
            actorUserId,
            occurredAtUtc,
            reason);
    }

    public void Cancel(
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? reason)
    {
        if (Status is IssueStatus.Closed or IssueStatus.Cancelled)
        {
            throw new DomainException(
                "The issue cannot be cancelled from its current status.");
        }

        Status = IssueStatus.Cancelled;

        AddHistory(
            IssueHistoryAction.Cancelled,
            actorUserId,
            occurredAtUtc,
            reason);
    }

    private void AddHistory(
        IssueHistoryAction action,
        Guid actorUserId,
        DateTimeOffset occurredAtUtc,
        string? note)
    {
        if (actorUserId == Guid.Empty)
        {
            throw new DomainException(
                "Issue history actor identifier is required.");
        }

        _history.Add(new IssueHistoryEntry(
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
            throw new DomainException("Issue number is required.");
        }

        if (number.Trim().Length > 100)
        {
            throw new DomainException(
                "Issue number cannot exceed 100 characters.");
        }
    }

    private static void ValidateDetails(
        string title,
        string description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Issue title is required.");
        }

        if (title.Trim().Length > 300)
        {
            throw new DomainException(
                "Issue title cannot exceed 300 characters.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Issue description is required.");
        }

        if (description.Trim().Length > 10000)
        {
            throw new DomainException(
                "Issue description cannot exceed 10000 characters.");
        }
    }

    private static string NormalizeNumber(string value) =>
        value.Trim().ToUpperInvariant();
}
