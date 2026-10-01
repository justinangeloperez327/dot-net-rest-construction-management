using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Messaging;

namespace Construction.Application.Attachments.DownloadAttachment;

public sealed record DownloadAttachmentQuery(
    Guid ProjectId,
    Guid AttachmentId) : IQuery<FileDownload>;
