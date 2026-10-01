using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Submittals.GetSubmittals;

public sealed class GetSubmittalsQueryHandler(
    ISubmittalRepository submittals,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetSubmittalsQuery, PagedResult<SubmittalSummaryResponse>>
{
    public async Task<Result<PagedResult<SubmittalSummaryResponse>>> HandleAsync(
        GetSubmittalsQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Submittals.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<SubmittalSummaryResponse>>(
                accessError);
        }

        var page = await submittals.GetPageAsync(
            query.ProjectId,
            query.Page,
            cancellationToken);

        return Result.Success(new PagedResult<SubmittalSummaryResponse>(
            page.Items.Select(SubmittalSummaryResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(
                query.Page.PageSize,
                1,
                PageRequest.MaximumPageSize),
            page.TotalCount));
    }
}
