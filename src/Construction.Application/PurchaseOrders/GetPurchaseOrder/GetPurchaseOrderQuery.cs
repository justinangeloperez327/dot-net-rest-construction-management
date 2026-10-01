using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseOrders.GetPurchaseOrder;

public sealed record GetPurchaseOrderQuery(
    Guid ProjectId,
    Guid PurchaseOrderId) : IQuery<PurchaseOrderResponse>;
