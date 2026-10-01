using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Inspections.GetInspections;

public sealed record GetInspectionsQuery(
    Guid ProjectId,
    PageRequest Page)
    : IQuery<PagedResult<InspectionSummaryResponse>>;
