using Construction.Domain.Attachments;

namespace Construction.Application.Attachments;

public sealed record AttachmentResponse(
    Guid Id,
    Guid ProjectId,
    AttachmentTargetType TargetType,
    Guid TargetId,
    string FileName,
    string ContentType,
    long Length,
    string? Description,
    Guid UploadedByUserId,
    DateTimeOffset UploadedAtUtc)
{
    public static AttachmentResponse FromDomain(Attachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);

        return new AttachmentResponse(
            attachment.Id,
            attachment.ProjectId,
            attachment.TargetType,
            attachment.TargetId,
            attachment.FileName,
            attachment.ContentType,
            attachment.Length,
            attachment.Description,
            attachment.UploadedByUserId,
            attachment.UploadedAtUtc);
    }
}
