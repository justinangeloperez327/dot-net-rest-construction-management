using Construction.Application.Common.Messaging;
using Construction.Application.PurchaseOrders.CreatePurchaseOrder;

namespace Construction.Application.PurchaseOrders.UpdatePurchaseOrder;

public sealed record UpdatePurchaseOrderCommand(
    Guid ProjectId,
    Guid PurchaseOrderId,
    Guid SupplierId,
    string CurrencyCode,
    DateOnly? ExpectedDeliveryDate,
    IReadOnlyCollection<PurchaseOrderItemInputModel> Items)
    : ICommand<PurchaseOrderResponse>;
