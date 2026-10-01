using Construction.Domain.Documents;

namespace Construction.Api.Contracts.Documents;

public sealed record UpdateDocumentRequest(
    string Title,
    DocumentCategory Category,
    string? Description);
