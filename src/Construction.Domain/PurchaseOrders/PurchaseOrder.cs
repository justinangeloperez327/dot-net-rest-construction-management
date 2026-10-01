using Construction.Domain.Common;

namespace Construction.Domain.PurchaseOrders;

public sealed class PurchaseOrder : AuditableAggregateRoot<Guid>
{
    private readonly List<PurchaseOrderItem> _items = [];

    private PurchaseOrder()
        : base(Guid.Empty)
    {
    }

    private PurchaseOrder(
        Guid id,
        Guid projectId,
        Guid supplierId,
        Guid? purchaseRequestId,
        string number,
        string currencyCode,
        DateOnly? expectedDeliveryDate,
        Guid createdByUserId)
        : base(id)
    {
        ProjectId = projectId;
        SupplierId = supplierId;
        PurchaseRequestId = purchaseRequestId;
        Number = number;
        NormalizedNumber = Normalize(number);
        CurrencyCode = currencyCode;
        ExpectedDeliveryDate = expectedDeliveryDate;
        CreatedByUserId = createdByUserId;
        Status = PurchaseOrderStatus.Draft;
    }

    public Guid ProjectId { get; private set; }
    public Guid SupplierId { get; private set; }
    public Guid? PurchaseRequestId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string NormalizedNumber { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public DateOnly? ExpectedDeliveryDate { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public Guid? IssuedByUserId { get; private set; }
    public DateTimeOffset? IssuedAtUtc { get; private set; }

    public IReadOnlyCollection<PurchaseOrderItem> Items => _items;

    public decimal Total => _items.Sum(item => item.LineTotal);

    public static PurchaseOrder Create(
        Guid projectId,
        Guid supplierId,
        Guid? purchaseRequestId,
        string number,
        string currencyCode,
        DateOnly? expectedDeliveryDate,
        Guid createdByUserId,
        IEnumerable<PurchaseOrderItemInput> items)
    {
        if (projectId == Guid.Empty
            || supplierId == Guid.Empty
            || createdByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Project, supplier, and creator identifiers are required.");
        }

        ValidateNumber(number);
        string currency = ValidateCurrency(currencyCode);

        var order = new PurchaseOrder(
            Guid.CreateVersion7(),
            projectId,
            supplierId,
            purchaseRequestId,
            number.Trim(),
            currency,
            expectedDeliveryDate,
            createdByUserId);

        order.ReplaceItems(items);
        return order;
    }

    public void Update(
        Guid supplierId,
        string currencyCode,
        DateOnly? expectedDeliveryDate,
        IEnumerable<PurchaseOrderItemInput> items)
    {
        if (Status != PurchaseOrderStatus.Draft)
        {
            throw new DomainException(
                "Only draft purchase orders can be updated.");
        }

        if (supplierId == Guid.Empty)
        {
            throw new DomainException(
                "Supplier identifier is required.");
        }

        SupplierId = supplierId;
        CurrencyCode = ValidateCurrency(currencyCode);
        ExpectedDeliveryDate = expectedDeliveryDate;
        ReplaceItems(items);
    }

    public void Issue(
        Guid issuedByUserId,
        DateTimeOffset issuedAtUtc)
    {
        if (Status != PurchaseOrderStatus.Draft)
        {
            throw new DomainException(
                "Only draft purchase orders can be issued.");
        }

        Status = PurchaseOrderStatus.Issued;
        IssuedByUserId = issuedByUserId;
        IssuedAtUtc = issuedAtUtc;
    }

    public void Receive(
        IEnumerable<PurchaseOrderReceiptInput> receipts)
    {
        if (Status is not PurchaseOrderStatus.Issued
            and not PurchaseOrderStatus.PartiallyDelivered)
        {
            throw new DomainException(
                "Deliveries can only be recorded for issued purchase orders.");
        }

        PurchaseOrderReceiptInput[] values = [.. receipts];

        if (values.Length == 0)
        {
            throw new DomainException(
                "At least one received item is required.");
        }

        foreach (PurchaseOrderReceiptInput receipt in values)
        {
            PurchaseOrderItem item =
                _items.SingleOrDefault(value =>
                    value.Id == receipt.ItemId)
                ?? throw new DomainException(
                    "Purchase order item was not found.");

            item.Receive(receipt.Quantity);
        }

        bool delivered = _items.All(item =>
            item.ReceivedQuantity == item.OrderedQuantity);

        Status = delivered
            ? PurchaseOrderStatus.Delivered
            : PurchaseOrderStatus.PartiallyDelivered;
    }

    public void Close()
    {
        if (Status != PurchaseOrderStatus.Delivered)
        {
            throw new DomainException(
                "Only fully delivered purchase orders can be closed.");
        }

        Status = PurchaseOrderStatus.Closed;
    }

    public void Cancel()
    {
        if (Status is PurchaseOrderStatus.Delivered
            or PurchaseOrderStatus.Closed
            or PurchaseOrderStatus.Cancelled)
        {
            throw new DomainException(
                "Purchase order cannot be cancelled in its current status.");
        }

        if (_items.Any(item => item.ReceivedQuantity > 0))
        {
            throw new DomainException(
                "Purchase orders with received quantities cannot be cancelled.");
        }

        Status = PurchaseOrderStatus.Cancelled;
    }

    private void ReplaceItems(IEnumerable<PurchaseOrderItemInput> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        PurchaseOrderItemInput[] values = [.. items];

        if (values.Length == 0)
        {
            throw new DomainException(
                "Purchase order must contain at least one item.");
        }

        _items.Clear();

        foreach (PurchaseOrderItemInput item in values)
        {
            _items.Add(new PurchaseOrderItem(
                Guid.CreateVersion7(),
                Id,
                item.Description,
                item.OrderedQuantity,
                item.Unit,
                item.UnitPrice));
        }
    }

    private static void ValidateNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                "Purchase order number is required.");
        }

        if (value.Trim().Length > 100)
        {
            throw new DomainException(
                "Purchase order number cannot exceed 100 characters.");
        }
    }

    private static string ValidateCurrency(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Trim().Length != 3)
        {
            throw new DomainException(
                "Currency code must contain three characters.");
        }

        return value.Trim().ToUpperInvariant();
    }

    private static string Normalize(string value) =>
        value.Trim().ToUpperInvariant();
}

public sealed record PurchaseOrderItemInput(
    string Description,
    decimal OrderedQuantity,
    string Unit,
    decimal UnitPrice);

public sealed record PurchaseOrderReceiptInput(
    Guid ItemId,
    decimal Quantity);
