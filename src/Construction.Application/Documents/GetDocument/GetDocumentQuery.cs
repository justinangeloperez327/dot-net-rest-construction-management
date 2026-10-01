using Construction.Application.Common.Messaging;

namespace Construction.Application.Documents.GetDocument;

public sealed record GetDocumentQuery(
    Guid ProjectId,
    Guid DocumentId) : IQuery<DocumentResponse>;
