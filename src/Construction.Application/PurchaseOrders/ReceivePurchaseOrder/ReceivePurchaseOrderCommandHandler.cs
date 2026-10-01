using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.PurchaseOrders;

namespace Construction.Application.PurchaseOrders.ReceivePurchaseOrder;

public sealed class ReceivePurchaseOrderCommandHandler(
    IPurchaseOrderRepository orders,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<ReceivePurchaseOrderCommand, PurchaseOrderResponse>
{
    public async Task<Result<PurchaseOrderResponse>> HandleAsync(
        ReceivePurchaseOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            command.ProjectId,
            Permissions.Procurement.ManageOrders,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PurchaseOrderResponse>(accessError);
        }

        var order = await orders.GetAsync(command.PurchaseOrderId, cancellationToken);

        if (order is null || order.ProjectId != command.ProjectId)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.NotFound("PurchaseOrders.NotFound", "The purchase order was not found."));
        }

        order.Receive(command.Receipts.Select(receipt =>
            new PurchaseOrderReceiptInput(receipt.ItemId, receipt.Quantity)));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseOrderResponse.FromDomain(order));
    }
}
