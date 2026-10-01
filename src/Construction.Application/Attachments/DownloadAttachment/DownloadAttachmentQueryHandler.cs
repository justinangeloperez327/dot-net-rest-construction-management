using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Attachments.DownloadAttachment;

public sealed class DownloadAttachmentQueryHandler(
    IAttachmentRepository attachments,
    IFileStorage fileStorage,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<DownloadAttachmentQuery, FileDownload>
{
    public async Task<Result<FileDownload>> HandleAsync(
        DownloadAttachmentQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Documents.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<FileDownload>(accessError);
        }

        var attachment = await attachments.GetAsync(
            query.AttachmentId,
            cancellationToken);

        if (attachment is null || attachment.ProjectId != query.ProjectId)
        {
            return Result.Failure<FileDownload>(
                ApplicationError.NotFound(
                    "Attachments.NotFound",
                    "The attachment was not found."));
        }

        Stream? content = await fileStorage.OpenReadAsync(
            attachment.StorageKey,
            cancellationToken);

        return content is null
            ? Result.Failure<FileDownload>(
                ApplicationError.NotFound(
                    "Attachments.FileNotFound",
                    "The attachment file is unavailable."))
            : Result.Success(new FileDownload(
                content,
                attachment.FileName,
                attachment.ContentType,
                attachment.Length));
    }
}
