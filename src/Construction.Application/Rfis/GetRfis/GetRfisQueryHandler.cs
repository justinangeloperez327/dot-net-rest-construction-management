using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Rfis.GetRfis;

public sealed class GetRfisQueryHandler(
    IRfiRepository rfis,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetRfisQuery, PagedResult<RfiSummaryResponse>>
{
    public async Task<Result<PagedResult<RfiSummaryResponse>>> HandleAsync(
        GetRfisQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Rfis.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<RfiSummaryResponse>>(accessError);
        }

        var page = await rfis.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        return Result.Success(new PagedResult<RfiSummaryResponse>(
            page.Items.Select(RfiSummaryResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(
                query.Page.PageSize,
                1,
                PageRequest.MaximumPageSize),
            page.TotalCount));
    }
}
