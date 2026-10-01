using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.PurchaseOrders.GetPurchaseOrders;

public sealed class GetPurchaseOrdersQueryHandler(
    IPurchaseOrderRepository orders,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetPurchaseOrdersQuery, PagedResult<PurchaseOrderSummaryResponse>>
{
    public async Task<Result<PagedResult<PurchaseOrderSummaryResponse>>> HandleAsync(
        GetPurchaseOrdersQuery query,
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
            return Result.Failure<PagedResult<PurchaseOrderSummaryResponse>>(accessError);
        }

        var page = await orders.GetPageAsync(query.ProjectId, query.Page, cancellationToken);

        return Result.Success(new PagedResult<PurchaseOrderSummaryResponse>(
            page.Items.Select(PurchaseOrderSummaryResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(query.Page.PageSize, 1, PageRequest.MaximumPageSize),
            page.TotalCount));
    }
}
