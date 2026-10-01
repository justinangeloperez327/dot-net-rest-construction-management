using Construction.Domain.PurchaseOrders;

namespace Construction.Application.PurchaseOrders;

public sealed record PurchaseOrderSummaryResponse(
    Guid Id,
    string Number,
    Guid SupplierId,
    Guid? PurchaseRequestId,
    string CurrencyCode,
    PurchaseOrderStatus Status,
    DateOnly? ExpectedDeliveryDate,
    decimal Total,
    DateTimeOffset CreatedAtUtc)
{
    public static PurchaseOrderSummaryResponse FromDomain(PurchaseOrder order) =>
        new(
            order.Id,
            order.Number,
            order.SupplierId,
            order.PurchaseRequestId,
            order.CurrencyCode,
            order.Status,
            order.ExpectedDeliveryDate,
            order.Total,
            order.CreatedAtUtc);
}
