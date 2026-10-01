using Construction.Domain.PurchaseOrders;

namespace Construction.Application.PurchaseOrders;

public sealed record PurchaseOrderResponse(
    Guid Id,
    Guid ProjectId,
    Guid SupplierId,
    Guid? PurchaseRequestId,
    string Number,
    string CurrencyCode,
    DateOnly? ExpectedDeliveryDate,
    Guid CreatedByUserId,
    PurchaseOrderStatus Status,
    Guid? IssuedByUserId,
    DateTimeOffset? IssuedAtUtc,
    decimal Total,
    IReadOnlyCollection<PurchaseOrderItemResponse> Items,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static PurchaseOrderResponse FromDomain(PurchaseOrder order) =>
        new(
            order.Id,
            order.ProjectId,
            order.SupplierId,
            order.PurchaseRequestId,
            order.Number,
            order.CurrencyCode,
            order.ExpectedDeliveryDate,
            order.CreatedByUserId,
            order.Status,
            order.IssuedByUserId,
            order.IssuedAtUtc,
            order.Total,
            order.Items.Select(PurchaseOrderItemResponse.FromDomain).ToArray(),
            order.CreatedAtUtc,
            order.LastModifiedAtUtc);
}

public sealed record PurchaseOrderItemResponse(
    Guid Id,
    string Description,
    decimal OrderedQuantity,
    decimal ReceivedQuantity,
    string Unit,
    decimal UnitPrice,
    decimal LineTotal)
{
    public static PurchaseOrderItemResponse FromDomain(PurchaseOrderItem item) =>
        new(
            item.Id,
            item.Description,
            item.OrderedQuantity,
            item.ReceivedQuantity,
            item.Unit,
            item.UnitPrice,
            item.LineTotal);
}
