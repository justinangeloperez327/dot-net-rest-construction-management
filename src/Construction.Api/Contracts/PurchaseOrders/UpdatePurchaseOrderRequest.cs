namespace Construction.Api.Contracts.PurchaseOrders;

public sealed record UpdatePurchaseOrderRequest(
    Guid SupplierId,
    string CurrencyCode,
    DateOnly? ExpectedDeliveryDate,
    IReadOnlyCollection<PurchaseOrderItemRequest> Items);
