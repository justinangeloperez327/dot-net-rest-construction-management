using Construction.Domain.Common;

namespace Construction.Domain.PurchaseOrders;

public sealed class PurchaseOrderItem : AuditableEntity<Guid>
{
    private PurchaseOrderItem()
        : base(Guid.Empty)
    {
    }

    internal PurchaseOrderItem(
        Guid id,
        Guid purchaseOrderId,
        string description,
        decimal orderedQuantity,
        string unit,
        decimal unitPrice)
        : base(id)
    {
        PurchaseOrderId = purchaseOrderId;
        Description = ValidateText(description, 500, "Item description");
        OrderedQuantity = ValidatePositive(orderedQuantity, "Ordered quantity");
        Unit = ValidateText(unit, 50, "Item unit");

        if (unitPrice < 0)
        {
            throw new DomainException(
                "Purchase order unit price cannot be negative.");
        }

        UnitPrice = unitPrice;
    }

    public Guid PurchaseOrderId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal OrderedQuantity { get; private set; }
    public decimal ReceivedQuantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal => OrderedQuantity * UnitPrice;

    internal void Receive(decimal quantity)
    {
        ValidatePositive(quantity, "Received quantity");

        if (ReceivedQuantity + quantity > OrderedQuantity)
        {
            throw new DomainException(
                "Received quantity cannot exceed ordered quantity.");
        }

        ReceivedQuantity += quantity;
    }

    private static decimal ValidatePositive(
        decimal value,
        string field)
    {
        if (value <= 0)
        {
            throw new DomainException(
                $"{field} must be greater than zero.");
        }

        return value;
    }

    private static string ValidateText(
        string value,
        int maximumLength,
        string field)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{field} is required.");
        }

        if (value.Trim().Length > maximumLength)
        {
            throw new DomainException(
                $"{field} cannot exceed {maximumLength} characters.");
        }

        return value.Trim();
    }
}
