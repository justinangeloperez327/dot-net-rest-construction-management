using Construction.Domain.Documents;

namespace Construction.Api.Contracts.Documents;

public sealed record CreateDocumentRequest(
    string Number,
    string Title,
    DocumentCategory Category,
    string? Description);
