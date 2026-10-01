namespace Construction.Api.Contracts.PurchaseOrders;

public sealed record CreatePurchaseOrderRequest(
    Guid SupplierId,
    Guid? PurchaseRequestId,
    string Number,
    string CurrencyCode,
    DateOnly? ExpectedDeliveryDate,
    IReadOnlyCollection<PurchaseOrderItemRequest> Items);

public sealed record PurchaseOrderItemRequest(
    string Description,
    decimal OrderedQuantity,
    string Unit,
    decimal UnitPrice);
