using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Issues.GetIssues;

public sealed record GetIssuesQuery(
    Guid ProjectId,
    PageRequest Page)
    : IQuery<PagedResult<IssueSummaryResponse>>;
