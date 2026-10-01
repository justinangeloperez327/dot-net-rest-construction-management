using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Documents.GetDocuments;

public sealed record GetDocumentsQuery(
    Guid ProjectId,
    PageRequest Page)
    : IQuery<PagedResult<DocumentSummaryResponse>>;
