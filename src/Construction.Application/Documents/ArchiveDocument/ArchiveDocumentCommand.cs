using Construction.Application.Common.Messaging;

namespace Construction.Application.Documents.ArchiveDocument;

public sealed record ArchiveDocumentCommand(
    Guid ProjectId,
    Guid DocumentId) : ICommand;
