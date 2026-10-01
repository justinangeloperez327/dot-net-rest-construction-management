using Construction.Domain.Common;
using Construction.Domain.PurchaseOrders;
using Xunit;

namespace Construction.Domain.Tests.PurchaseOrders;

public sealed class PurchaseOrderTests
{
    [Fact]
    public void Receive_TracksPartialAndFullDelivery()
    {
        var order = PurchaseOrder.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "PO-001",
            "AED",
            new DateOnly(2026, 2, 1),
            Guid.NewGuid(),
            [
                new PurchaseOrderItemInput(
                    "Cement",
                    100m,
                    "bag",
                    25m)
            ]);

        order.Issue(
            Guid.NewGuid(),
            new DateTimeOffset(
                2026,
                1,
                1,
                8,
                0,
                0,
                TimeSpan.Zero));

        PurchaseOrderItem item = Assert.Single(order.Items);

        order.Receive(
            [new PurchaseOrderReceiptInput(item.Id, 40m)]);

        Assert.Equal(
            PurchaseOrderStatus.PartiallyDelivered,
            order.Status);
        Assert.Equal(40m, item.ReceivedQuantity);

        Assert.Throws<DomainException>(
            () => order.Cancel());

        order.Receive(
            [new PurchaseOrderReceiptInput(item.Id, 60m)]);

        Assert.Equal(
            PurchaseOrderStatus.Delivered,
            order.Status);

        order.Close();

        Assert.Equal(
            PurchaseOrderStatus.Closed,
            order.Status);
    }
}
