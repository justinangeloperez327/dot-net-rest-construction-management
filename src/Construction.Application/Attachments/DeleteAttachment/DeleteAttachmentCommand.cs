using Construction.Application.Common.Messaging;

namespace Construction.Application.Attachments.DeleteAttachment;

public sealed record DeleteAttachmentCommand(
    Guid ProjectId,
    Guid AttachmentId) : ICommand;
