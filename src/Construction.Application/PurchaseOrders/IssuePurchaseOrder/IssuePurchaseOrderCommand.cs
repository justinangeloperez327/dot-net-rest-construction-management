using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseOrders.IssuePurchaseOrder;

public sealed record IssuePurchaseOrderCommand(
    Guid ProjectId,
    Guid PurchaseOrderId) : ICommand<PurchaseOrderResponse>;
