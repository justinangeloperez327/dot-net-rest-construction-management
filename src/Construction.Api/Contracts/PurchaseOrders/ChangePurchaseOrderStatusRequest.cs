using Construction.Domain.PurchaseOrders;

namespace Construction.Api.Contracts.PurchaseOrders;

public sealed record ChangePurchaseOrderStatusRequest(
    PurchaseOrderStatus Status);
