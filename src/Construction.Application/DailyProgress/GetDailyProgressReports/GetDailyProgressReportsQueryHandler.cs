using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.DailyProgress.GetDailyProgressReports;

public sealed class GetDailyProgressReportsQueryHandler(
    IDailyProgressRepository reports,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetDailyProgressReportsQuery, PagedResult<DailyProgressSummaryResponse>>
{
    public async Task<Result<PagedResult<DailyProgressSummaryResponse>>> HandleAsync(
        GetDailyProgressReportsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.DailyProgress.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<DailyProgressSummaryResponse>>(
                accessError);
        }

        var page = await reports.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        var response = new PagedResult<DailyProgressSummaryResponse>(
            page.Items
                .Select(DailyProgressSummaryResponse.FromDomain)
                .ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(
                query.Page.PageSize,
                1,
                PageRequest.MaximumPageSize),
            page.TotalCount);

        return Result.Success(response);
    }
}
