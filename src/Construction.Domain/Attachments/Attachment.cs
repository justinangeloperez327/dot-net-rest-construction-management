using Construction.Domain.Common;

namespace Construction.Domain.Attachments;

public sealed class Attachment : AuditableEntity<Guid>
{
    private Attachment()
        : base(Guid.Empty)
    {
    }

    private Attachment(
        Guid id,
        Guid projectId,
        AttachmentTargetType targetType,
        Guid targetId,
        string fileName,
        string contentType,
        long length,
        string storageKey,
        string? description,
        Guid uploadedByUserId,
        DateTimeOffset uploadedAtUtc)
        : base(id)
    {
        ProjectId = projectId;
        TargetType = targetType;
        TargetId = targetId;
        FileName = fileName;
        ContentType = contentType;
        Length = length;
        StorageKey = storageKey;
        Description = NormalizeOptional(description);
        UploadedByUserId = uploadedByUserId;
        UploadedAtUtc = uploadedAtUtc;
    }

    public Guid ProjectId { get; private set; }

    public AttachmentTargetType TargetType { get; private set; }

    public Guid TargetId { get; private set; }

    public string FileName { get; private set; } = string.Empty;

    public string ContentType { get; private set; } = string.Empty;

    public long Length { get; private set; }

    public string StorageKey { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid UploadedByUserId { get; private set; }

    public DateTimeOffset UploadedAtUtc { get; private set; }

    public static Attachment Create(
        Guid projectId,
        AttachmentTargetType targetType,
        Guid targetId,
        string fileName,
        string contentType,
        long length,
        string storageKey,
        string? description,
        Guid uploadedByUserId,
        DateTimeOffset uploadedAtUtc)
    {
        if (projectId == Guid.Empty || targetId == Guid.Empty)
        {
            throw new DomainException(
                "Project and attachment target identifiers are required.");
        }

        string safeFileName = Path.GetFileName(fileName);

        if (string.IsNullOrWhiteSpace(safeFileName)
            || safeFileName.Length > 255)
        {
            throw new DomainException(
                "Attachment file name is required and cannot exceed 255 characters.");
        }

        if (string.IsNullOrWhiteSpace(contentType)
            || contentType.Trim().Length > 200)
        {
            throw new DomainException(
                "Attachment content type is required and cannot exceed 200 characters.");
        }

        if (length <= 0)
        {
            throw new DomainException(
                "Attachment file cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(storageKey)
            || storageKey.Length > 500)
        {
            throw new DomainException(
                "Attachment storage key is invalid.");
        }

        if (description?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Attachment description cannot exceed 2000 characters.");
        }

        if (uploadedByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Attachment uploader identifier is required.");
        }

        return new Attachment(
            Guid.CreateVersion7(),
            projectId,
            targetType,
            targetId,
            safeFileName,
            contentType.Trim(),
            length,
            storageKey,
            description,
            uploadedByUserId,
            uploadedAtUtc);
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
