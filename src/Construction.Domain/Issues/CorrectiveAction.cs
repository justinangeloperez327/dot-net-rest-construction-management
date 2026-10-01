using Construction.Domain.Common;

namespace Construction.Domain.Issues;

public sealed class CorrectiveAction : AuditableEntity<Guid>
{
    private CorrectiveAction()
        : base(Guid.Empty)
    {
    }

    internal CorrectiveAction(
        Guid id,
        Guid issueId,
        string description,
        Guid? responsibleUserId,
        DateOnly? dueDate)
        : base(id)
    {
        IssueId = issueId;
        Description = ValidateDescription(description);
        ResponsibleUserId = responsibleUserId;
        DueDate = dueDate;
        Status = CorrectiveActionStatus.Pending;
    }

    public Guid IssueId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public Guid? ResponsibleUserId { get; private set; }

    public DateOnly? DueDate { get; private set; }

    public CorrectiveActionStatus Status { get; private set; }

    public string? CompletionNotes { get; private set; }

    public Guid? CompletedByUserId { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    internal void Update(
        string description,
        Guid? responsibleUserId,
        DateOnly? dueDate)
    {
        if (Status is CorrectiveActionStatus.Completed
            or CorrectiveActionStatus.Cancelled)
        {
            throw new DomainException(
                "Completed or cancelled corrective actions cannot be updated.");
        }

        Description = ValidateDescription(description);
        ResponsibleUserId = responsibleUserId;
        DueDate = dueDate;
    }

    internal void Start()
    {
        if (Status != CorrectiveActionStatus.Pending)
        {
            throw new DomainException(
                "Only pending corrective actions can be started.");
        }

        Status = CorrectiveActionStatus.InProgress;
    }

    internal void Complete(
        string completionNotes,
        Guid completedByUserId,
        DateTimeOffset completedAtUtc)
    {
        if (Status is CorrectiveActionStatus.Completed
            or CorrectiveActionStatus.Cancelled)
        {
            throw new DomainException(
                "The corrective action cannot be completed from its current status.");
        }

        if (string.IsNullOrWhiteSpace(completionNotes))
        {
            throw new DomainException(
                "Corrective action completion notes are required.");
        }

        if (completionNotes.Trim().Length > 4000)
        {
            throw new DomainException(
                "Corrective action completion notes cannot exceed 4000 characters.");
        }

        Status = CorrectiveActionStatus.Completed;
        CompletionNotes = completionNotes.Trim();
        CompletedByUserId = completedByUserId;
        CompletedAtUtc = completedAtUtc;
    }

    internal void Cancel()
    {
        if (Status == CorrectiveActionStatus.Completed)
        {
            throw new DomainException(
                "Completed corrective actions cannot be cancelled.");
        }

        Status = CorrectiveActionStatus.Cancelled;
    }

    private static string ValidateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(
                "Corrective action description is required.");
        }

        if (description.Trim().Length > 4000)
        {
            throw new DomainException(
                "Corrective action description cannot exceed 4000 characters.");
        }

        return description.Trim();
    }
}
