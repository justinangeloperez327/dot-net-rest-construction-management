using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.PurchaseRequests.GetPurchaseRequests;

public sealed class GetPurchaseRequestsQueryHandler(
    IPurchaseRequestRepository requests,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetPurchaseRequestsQuery, PagedResult<PurchaseRequestSummaryResponse>>
{
    public async Task<Result<PagedResult<PurchaseRequestSummaryResponse>>> HandleAsync(
        GetPurchaseRequestsQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            query.ProjectId,
            Permissions.Procurement.View,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PagedResult<PurchaseRequestSummaryResponse>>(accessError);
        }

        var page = await requests.GetPageAsync(query.ProjectId, query.Page, cancellationToken);

        return Result.Success(new PagedResult<PurchaseRequestSummaryResponse>(
            page.Items.Select(PurchaseRequestSummaryResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(query.Page.PageSize, 1, PageRequest.MaximumPageSize),
            page.TotalCount));
    }
}
