namespace Construction.Api.Contracts.PurchaseOrders;

public sealed record ReceivePurchaseOrderRequest(
    IReadOnlyCollection<PurchaseOrderReceiptRequest> Receipts);

public sealed record PurchaseOrderReceiptRequest(
    Guid ItemId,
    decimal Quantity);
