using Construction.Domain.Attachments;

namespace Construction.Api.Contracts.Documents;

public sealed class UploadAttachmentRequest
{
    public AttachmentTargetType TargetType { get; init; }

    public Guid TargetId { get; init; }

    public string? Description { get; init; }

    public required IFormFile File { get; init; }
}
