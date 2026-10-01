using Construction.Application.Abstractions.Files;
using Construction.Application.Common.Messaging;

namespace Construction.Application.Documents.DownloadDocumentRevision;

public sealed record DownloadDocumentRevisionQuery(
    Guid ProjectId,
    Guid DocumentId,
    Guid RevisionId) : IQuery<FileDownload>;
