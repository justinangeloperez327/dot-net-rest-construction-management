using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.PurchaseOrders;
using Construction.Domain.Suppliers;

namespace Construction.Application.PurchaseOrders.UpdatePurchaseOrder;

public sealed class UpdatePurchaseOrderCommandHandler(
    IPurchaseOrderRepository orders,
    ISupplierRepository suppliers,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdatePurchaseOrderCommand, PurchaseOrderResponse>
{
    public async Task<Result<PurchaseOrderResponse>> HandleAsync(
        UpdatePurchaseOrderCommand command,
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

        Supplier? supplier = await suppliers.GetAsync(command.SupplierId, cancellationToken);

        if (supplier is null || supplier.Status != SupplierStatus.Active)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.Validation("PurchaseOrders.InvalidSupplier", "Purchase order supplier must be active."));
        }

        order.Update(
            command.SupplierId,
            command.CurrencyCode,
            command.ExpectedDeliveryDate,
            command.Items.Select(item => new PurchaseOrderItemInput(
                item.Description,
                item.OrderedQuantity,
                item.Unit,
                item.UnitPrice)));

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseOrderResponse.FromDomain(order));
    }
}
