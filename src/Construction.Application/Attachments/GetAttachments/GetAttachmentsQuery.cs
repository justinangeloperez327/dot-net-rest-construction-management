using Construction.Application.Common.Messaging;
using Construction.Domain.Attachments;

namespace Construction.Application.Attachments.GetAttachments;

public sealed record GetAttachmentsQuery(
    Guid ProjectId,
    AttachmentTargetType TargetType,
    Guid TargetId)
    : IQuery<IReadOnlyCollection<AttachmentResponse>>;
