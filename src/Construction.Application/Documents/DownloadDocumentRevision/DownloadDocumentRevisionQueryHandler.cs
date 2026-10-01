using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Documents.DownloadDocumentRevision;

public sealed class DownloadDocumentRevisionQueryHandler(
    IDocumentRepository documents,
    IFileStorage fileStorage,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<DownloadDocumentRevisionQuery, FileDownload>
{
    public async Task<Result<FileDownload>> HandleAsync(
        DownloadDocumentRevisionQuery query,
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

        var document = await documents.GetAsync(
            query.DocumentId,
            cancellationToken);

        if (document is null || document.ProjectId != query.ProjectId)
        {
            return Result.Failure<FileDownload>(
                ApplicationError.NotFound(
                    "Documents.NotFound",
                    "The document was not found."));
        }

        var revision = document.Revisions.SingleOrDefault(
            item => item.Id == query.RevisionId);

        if (revision is null)
        {
            return Result.Failure<FileDownload>(
                ApplicationError.NotFound(
                    "Documents.RevisionNotFound",
                    "The document revision was not found."));
        }

        Stream? content = await fileStorage.OpenReadAsync(
            revision.StorageKey,
            cancellationToken);

        return content is null
            ? Result.Failure<FileDownload>(
                ApplicationError.NotFound(
                    "Documents.FileNotFound",
                    "The document revision file is unavailable."))
            : Result.Success(new FileDownload(
                content,
                revision.FileName,
                revision.ContentType,
                revision.Length));
    }
}
