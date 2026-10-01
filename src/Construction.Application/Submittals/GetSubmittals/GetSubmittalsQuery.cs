using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Submittals.GetSubmittals;

public sealed record GetSubmittalsQuery(
    Guid ProjectId,
    PageRequest Page)
    : IQuery<PagedResult<SubmittalSummaryResponse>>;
