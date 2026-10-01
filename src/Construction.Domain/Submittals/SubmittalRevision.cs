using Construction.Domain.Common;

namespace Construction.Domain.Submittals;

public sealed class SubmittalRevision : AuditableEntity<Guid>
{
    private SubmittalRevision()
        : base(Guid.Empty)
    {
    }

    internal SubmittalRevision(
        Guid id,
        Guid submittalId,
        int versionNumber,
        string revisionCode,
        string? description,
        Guid createdByUserId)
        : base(id)
    {
        SubmittalId = submittalId;
        VersionNumber = versionNumber;
        RevisionCode = revisionCode;
        Description = NormalizeOptional(description);
        CreatedByUserId = createdByUserId;
        Status = SubmittalRevisionStatus.Draft;
        IsCurrent = true;
    }

    public Guid SubmittalId { get; private set; }

    public int VersionNumber { get; private set; }

    public string RevisionCode { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public SubmittalRevisionStatus Status { get; private set; }

    public bool IsCurrent { get; private set; }

    public Guid? SubmittedByUserId { get; private set; }

    public DateTimeOffset? SubmittedAtUtc { get; private set; }

    public DateOnly? ReviewDueDate { get; private set; }

    public Guid? ReviewedByUserId { get; private set; }

    public DateTimeOffset? ReviewedAtUtc { get; private set; }

    public string? ReviewRemarks { get; private set; }

    internal void Submit(
        Guid submittedByUserId,
        DateTimeOffset submittedAtUtc,
        DateOnly? reviewDueDate)
    {
        if (Status != SubmittalRevisionStatus.Draft)
        {
            throw new DomainException(
                "Only draft submittal revisions can be submitted.");
        }

        Status = SubmittalRevisionStatus.Submitted;
        SubmittedByUserId = submittedByUserId;
        SubmittedAtUtc = submittedAtUtc;
        ReviewDueDate = reviewDueDate;
    }

    internal void StartReview()
    {
        if (Status != SubmittalRevisionStatus.Submitted)
        {
            throw new DomainException(
                "Only submitted revisions can enter review.");
        }

        Status = SubmittalRevisionStatus.UnderReview;
    }

    internal void Review(
        SubmittalRevisionStatus outcome,
        Guid reviewedByUserId,
        DateTimeOffset reviewedAtUtc,
        string? remarks)
    {
        if (Status is not SubmittalRevisionStatus.Submitted
            and not SubmittalRevisionStatus.UnderReview)
        {
            throw new DomainException(
                "Only submitted or under-review revisions can be reviewed.");
        }

        if (outcome is not SubmittalRevisionStatus.Approved
            and not SubmittalRevisionStatus.ApprovedWithComments
            and not SubmittalRevisionStatus.Rejected)
        {
            throw new DomainException(
                "The requested review outcome is invalid.");
        }

        if (outcome is SubmittalRevisionStatus.ApprovedWithComments
            or SubmittalRevisionStatus.Rejected)
        {
            if (string.IsNullOrWhiteSpace(remarks))
            {
                throw new DomainException(
                    "Review remarks are required for this outcome.");
            }
        }

        if (remarks?.Trim().Length > 4000)
        {
            throw new DomainException(
                "Review remarks cannot exceed 4000 characters.");
        }

        Status = outcome;
        ReviewedByUserId = reviewedByUserId;
        ReviewedAtUtc = reviewedAtUtc;
        ReviewRemarks = NormalizeOptional(remarks);
    }

    internal void Supersede() => IsCurrent = false;

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
