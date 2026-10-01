using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.DailyProgress.GetDailyProgressReports;

public sealed record GetDailyProgressReportsQuery(
    Guid ProjectId,
    PageRequest Page)
    : IQuery<PagedResult<DailyProgressSummaryResponse>>;
