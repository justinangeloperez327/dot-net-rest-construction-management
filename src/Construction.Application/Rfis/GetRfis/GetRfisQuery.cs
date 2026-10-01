using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Rfis.GetRfis;

public sealed record GetRfisQuery(
    Guid ProjectId,
    PageRequest Page)
    : IQuery<PagedResult<RfiSummaryResponse>>;
