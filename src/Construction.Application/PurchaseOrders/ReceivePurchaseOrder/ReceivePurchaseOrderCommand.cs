using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseOrders.ReceivePurchaseOrder;

public sealed record ReceivePurchaseOrderCommand(
    Guid ProjectId,
    Guid PurchaseOrderId,
    IReadOnlyCollection<PurchaseOrderReceiptInputModel> Receipts)
    : ICommand<PurchaseOrderResponse>;

public sealed record PurchaseOrderReceiptInputModel(
    Guid ItemId,
    decimal Quantity);
