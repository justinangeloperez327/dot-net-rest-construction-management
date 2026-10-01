using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseOrders.CreatePurchaseOrder;

public sealed record CreatePurchaseOrderCommand(
    Guid ProjectId,
    Guid SupplierId,
    Guid? PurchaseRequestId,
    string Number,
    string CurrencyCode,
    DateOnly? ExpectedDeliveryDate,
    IReadOnlyCollection<PurchaseOrderItemInputModel> Items)
    : ICommand<PurchaseOrderResponse>;

public sealed record PurchaseOrderItemInputModel(
    string Description,
    decimal OrderedQuantity,
    string Unit,
    decimal UnitPrice);
