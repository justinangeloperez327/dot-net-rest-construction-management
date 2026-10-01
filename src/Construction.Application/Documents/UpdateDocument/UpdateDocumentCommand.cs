using Construction.Application.Common.Messaging;
using Construction.Domain.Documents;

namespace Construction.Application.Documents.UpdateDocument;

public sealed record UpdateDocumentCommand(
    Guid ProjectId,
    Guid DocumentId,
    string Title,
    DocumentCategory Category,
    string? Description) : ICommand<DocumentResponse>;
