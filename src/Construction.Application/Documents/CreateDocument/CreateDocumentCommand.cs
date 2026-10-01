using Construction.Application.Common.Messaging;
using Construction.Domain.Documents;

namespace Construction.Application.Documents.CreateDocument;

public sealed record CreateDocumentCommand(
    Guid ProjectId,
    string Number,
    string Title,
    DocumentCategory Category,
    string? Description) : ICommand<DocumentResponse>;
