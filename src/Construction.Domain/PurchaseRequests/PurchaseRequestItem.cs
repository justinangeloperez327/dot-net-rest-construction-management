using Construction.Domain.Common;

namespace Construction.Domain.PurchaseRequests;

public sealed class PurchaseRequestItem : AuditableEntity<Guid>
{
    private PurchaseRequestItem()
        : base(Guid.Empty)
    {
    }

    internal PurchaseRequestItem(
        Guid id,
        Guid purchaseRequestId,
        string description,
        decimal quantity,
        string unit,
        decimal? estimatedUnitCost)
        : base(id)
    {
        PurchaseRequestId = purchaseRequestId;
        Description = ValidateText(description, 500, "Item description");
        Quantity = ValidateQuantity(quantity);
        Unit = ValidateText(unit, 50, "Item unit");
        EstimatedUnitCost = ValidateCost(estimatedUnitCost);
    }

    public Guid PurchaseRequestId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal? EstimatedUnitCost { get; private set; }

    private static decimal ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException(
                "Purchase request item quantity must be greater than zero.");
        }

        return quantity;
    }

    private static decimal? ValidateCost(decimal? cost)
    {
        if (cost < 0)
        {
            throw new DomainException(
                "Estimated unit cost cannot be negative.");
        }

        return cost;
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
