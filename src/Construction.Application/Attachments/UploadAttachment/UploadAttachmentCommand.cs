using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Messaging;
using Construction.Domain.Attachments;

namespace Construction.Application.Attachments.UploadAttachment;

public sealed record UploadAttachmentCommand(
    Guid ProjectId,
    AttachmentTargetType TargetType,
    Guid TargetId,
    string? Description,
    FileUpload File) : ICommand<AttachmentResponse>;
