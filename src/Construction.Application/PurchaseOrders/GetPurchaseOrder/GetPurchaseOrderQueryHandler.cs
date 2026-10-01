using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.PurchaseOrders.GetPurchaseOrder;

public sealed class GetPurchaseOrderQueryHandler(
    IPurchaseOrderRepository orders,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetPurchaseOrderQuery, PurchaseOrderResponse>
{
    public async Task<Result<PurchaseOrderResponse>> HandleAsync(
        GetPurchaseOrderQuery query,
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
            return Result.Failure<PurchaseOrderResponse>(accessError);
        }

        var order = await orders.GetAsync(query.PurchaseOrderId, cancellationToken);

        return order is null || order.ProjectId != query.ProjectId
            ? Result.Failure<PurchaseOrderResponse>(
                ApplicationError.NotFound("PurchaseOrders.NotFound", "The purchase order was not found."))
            : Result.Success(PurchaseOrderResponse.FromDomain(order));
    }
}
