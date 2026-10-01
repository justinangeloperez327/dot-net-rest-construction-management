namespace Construction.Domain.PurchaseOrders;

public enum PurchaseOrderStatus
{
    Draft = 0,
    Issued = 1,
    PartiallyDelivered = 2,
    Delivered = 3,
    Closed = 4,
    Cancelled = 5
}
