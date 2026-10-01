using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Issues.GetIssues;

public sealed class GetIssuesQueryHandler(
    IIssueRepository issues,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetIssuesQuery, PagedResult<IssueSummaryResponse>>
{
    public async Task<Result<PagedResult<IssueSummaryResponse>>> HandleAsync(
        GetIssuesQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Issues.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<IssueSummaryResponse>>(
                accessError);
        }

        var page = await issues.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        return Result.Success(
            new PagedResult<IssueSummaryResponse>(
                page.Items.Select(IssueSummaryResponse.FromDomain).ToArray(),
                Math.Max(1, query.Page.PageNumber),
                Math.Clamp(
                    query.Page.PageSize,
                    1,
                    PageRequest.MaximumPageSize),
                page.TotalCount));
    }
}
