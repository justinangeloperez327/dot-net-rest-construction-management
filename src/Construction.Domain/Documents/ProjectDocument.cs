using Construction.Domain.Common;

namespace Construction.Domain.Documents;

public sealed class ProjectDocument : AuditableAggregateRoot<Guid>
{
    private readonly List<DocumentRevision> _revisions = [];

    private ProjectDocument()
        : base(Guid.Empty)
    {
    }

    private ProjectDocument(
        Guid id,
        Guid projectId,
        string number,
        string title,
        DocumentCategory category,
        string? description,
        Guid createdByUserId)
        : base(id)
    {
        ProjectId = projectId;
        Number = number;
        NormalizedNumber = Normalize(number);
        Title = title;
        Category = category;
        Description = NormalizeOptional(description);
        CreatedByUserId = createdByUserId;
        Status = DocumentStatus.Active;
    }

    public Guid ProjectId { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public string NormalizedNumber { get; private set; } = string.Empty;

    public string Title { get; private set; } = string.Empty;

    public DocumentCategory Category { get; private set; }

    public string? Description { get; private set; }

    public Guid CreatedByUserId { get; private set; }

    public DocumentStatus Status { get; private set; }

    public int CurrentVersionNumber { get; private set; }

    public IReadOnlyCollection<DocumentRevision> Revisions => _revisions;

    public static ProjectDocument Create(
        Guid projectId,
        string number,
        string title,
        DocumentCategory category,
        string? description,
        Guid createdByUserId)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project identifier is required.");
        }

        if (createdByUserId == Guid.Empty)
        {
            throw new DomainException("Document creator identifier is required.");
        }

        ValidateNumber(number);
        ValidateTitle(title);
        ValidateDescription(description);

        return new ProjectDocument(
            Guid.CreateVersion7(),
            projectId,
            number.Trim(),
            title.Trim(),
            category,
            description,
            createdByUserId);
    }

    public void Update(
        string title,
        DocumentCategory category,
        string? description)
    {
        EnsureActive();
        ValidateTitle(title);
        ValidateDescription(description);

        Title = title.Trim();
        Category = category;
        Description = NormalizeOptional(description);
    }

    public DocumentRevision AddRevision(
        string revisionCode,
        string fileName,
        string contentType,
        long length,
        string storageKey,
        Guid uploadedByUserId,
        DateTimeOffset uploadedAtUtc,
        string? notes)
    {
        EnsureActive();
        ValidateRevision(
            revisionCode,
            fileName,
            contentType,
            length,
            storageKey,
            uploadedByUserId,
            notes);

        if (_revisions.Any(revision =>
            string.Equals(
                revision.RevisionCode,
                revisionCode.Trim(),
                StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException(
                "The document revision code already exists.");
        }

        foreach (DocumentRevision revision in _revisions.Where(r => r.IsCurrent))
        {
            revision.Supersede();
        }

        int versionNumber = CurrentVersionNumber + 1;

        var newRevision = new DocumentRevision(
            Guid.CreateVersion7(),
            Id,
            versionNumber,
            revisionCode.Trim(),
            Path.GetFileName(fileName.Trim()),
            contentType.Trim(),
            length,
            storageKey,
            uploadedByUserId,
            uploadedAtUtc,
            notes);

        _revisions.Add(newRevision);
        CurrentVersionNumber = versionNumber;

        return newRevision;
    }

    public void Archive()
    {
        Status = DocumentStatus.Archived;
    }

    private void EnsureActive()
    {
        if (Status == DocumentStatus.Archived)
        {
            throw new DomainException(
                "Archived documents cannot be modified.");
        }
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("Document number is required.");
        }

        if (number.Trim().Length > 100)
        {
            throw new DomainException(
                "Document number cannot exceed 100 characters.");
        }
    }

    private static void ValidateTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Document title is required.");
        }

        if (title.Trim().Length > 300)
        {
            throw new DomainException(
                "Document title cannot exceed 300 characters.");
        }
    }

    private static void ValidateDescription(string? description)
    {
        if (description?.Trim().Length > 4000)
        {
            throw new DomainException(
                "Document description cannot exceed 4000 characters.");
        }
    }

    private static void ValidateRevision(
        string revisionCode,
        string fileName,
        string contentType,
        long length,
        string storageKey,
        Guid uploadedByUserId,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(revisionCode)
            || revisionCode.Trim().Length > 50)
        {
            throw new DomainException(
                "Revision code is required and cannot exceed 50 characters.");
        }

        string safeFileName = Path.GetFileName(fileName);

        if (string.IsNullOrWhiteSpace(safeFileName)
            || safeFileName.Length > 255)
        {
            throw new DomainException(
                "File name is required and cannot exceed 255 characters.");
        }

        if (string.IsNullOrWhiteSpace(contentType)
            || contentType.Trim().Length > 200)
        {
            throw new DomainException(
                "Content type is required and cannot exceed 200 characters.");
        }

        if (length <= 0)
        {
            throw new DomainException(
                "Document revision file cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(storageKey)
            || storageKey.Length > 500)
        {
            throw new DomainException(
                "Storage key is required and cannot exceed 500 characters.");
        }

        if (uploadedByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Uploader identifier is required.");
        }

        if (notes?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Revision notes cannot exceed 2000 characters.");
        }
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
