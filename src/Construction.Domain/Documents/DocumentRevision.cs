using Construction.Domain.Common;

namespace Construction.Domain.Documents;

public sealed class DocumentRevision : AuditableEntity<Guid>
{
    private DocumentRevision()
        : base(Guid.Empty)
    {
    }

    internal DocumentRevision(
        Guid id,
        Guid documentId,
        int versionNumber,
        string revisionCode,
        string fileName,
        string contentType,
        long length,
        string storageKey,
        Guid uploadedByUserId,
        DateTimeOffset uploadedAtUtc,
        string? notes)
        : base(id)
    {
        DocumentId = documentId;
        VersionNumber = versionNumber;
        RevisionCode = revisionCode;
        FileName = fileName;
        ContentType = contentType;
        Length = length;
        StorageKey = storageKey;
        UploadedByUserId = uploadedByUserId;
        UploadedAtUtc = uploadedAtUtc;
        Notes = NormalizeOptional(notes);
        IsCurrent = true;
    }

    public Guid DocumentId { get; private set; }

    public int VersionNumber { get; private set; }

    public string RevisionCode { get; private set; } = string.Empty;

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long Length { get; private set; }

    public string StorageKey { get; private set; } = string.Empty;

    public Guid UploadedByUserId { get; private set; }

    public DateTimeOffset UploadedAtUtc { get; private set; }

    public string? Notes { get; private set; }

    public bool IsCurrent { get; private set; }

    internal void Supersede() => IsCurrent = false;

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
