using Construction.Application.Common.Messaging;
using Construction.Domain.PurchaseOrders;

namespace Construction.Application.PurchaseOrders.ChangePurchaseOrderStatus;

public sealed record ChangePurchaseOrderStatusCommand(
    Guid ProjectId,
    Guid PurchaseOrderId,
    PurchaseOrderStatus Status) : ICommand<PurchaseOrderResponse>;
