using Construction.Domain.Common;

namespace Construction.Domain.PurchaseRequests;

public sealed class PurchaseRequest : AuditableAggregateRoot<Guid>
{
    private readonly List<PurchaseRequestItem> _items = [];

    private PurchaseRequest()
        : base(Guid.Empty)
    {
    }

    private PurchaseRequest(
        Guid id,
        Guid projectId,
        string number,
        string title,
        string currencyCode,
        Guid requestedByUserId)
        : base(id)
    {
        ProjectId = projectId;
        Number = number;
        NormalizedNumber = Normalize(number);
        Title = title;
        CurrencyCode = currencyCode;
        RequestedByUserId = requestedByUserId;
        Status = PurchaseRequestStatus.Draft;
    }

    public Guid ProjectId { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public string NormalizedNumber { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string CurrencyCode { get; private set; } = string.Empty;
    public Guid RequestedByUserId { get; private set; }
    public PurchaseRequestStatus Status { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }
    public DateTimeOffset? ReviewedAtUtc { get; private set; }
    public string? ReviewRemarks { get; private set; }

    public IReadOnlyCollection<PurchaseRequestItem> Items => _items;

    public decimal EstimatedTotal =>
        _items.Sum(item =>
            item.Quantity * (item.EstimatedUnitCost ?? 0m));

    public static PurchaseRequest Create(
        Guid projectId,
        string number,
        string title,
        string currencyCode,
        Guid requestedByUserId,
        IEnumerable<PurchaseRequestItemInput> items)
    {
        if (projectId == Guid.Empty || requestedByUserId == Guid.Empty)
        {
            throw new DomainException(
                "Project and requester identifiers are required.");
        }

        ValidateNumber(number);
        ValidateTitle(title);
        string normalizedCurrency = ValidateCurrency(currencyCode);

        var request = new PurchaseRequest(
            Guid.CreateVersion7(),
            projectId,
            number.Trim(),
            title.Trim(),
            normalizedCurrency,
            requestedByUserId);

        request.ReplaceItems(items);
        return request;
    }

    public void Update(
        string title,
        string currencyCode,
        IEnumerable<PurchaseRequestItemInput> items)
    {
        if (Status is not PurchaseRequestStatus.Draft
            and not PurchaseRequestStatus.Rejected)
        {
            throw new DomainException(
                "Only draft or rejected purchase requests can be updated.");
        }

        ValidateTitle(title);

        Title = title.Trim();
        CurrencyCode = ValidateCurrency(currencyCode);
        ReplaceItems(items);

        if (Status == PurchaseRequestStatus.Rejected)
        {
            Status = PurchaseRequestStatus.Draft;
            ReviewedByUserId = null;
            ReviewedAtUtc = null;
            ReviewRemarks = null;
        }
    }

    public void Submit()
    {
        if (Status != PurchaseRequestStatus.Draft)
        {
            throw new DomainException(
                "Only draft purchase requests can be submitted.");
        }

        if (_items.Count == 0)
        {
            throw new DomainException(
                "Purchase request must contain at least one item.");
        }

        Status = PurchaseRequestStatus.Submitted;
    }

    public void Approve(
        Guid reviewerUserId,
        DateTimeOffset reviewedAtUtc,
        string? remarks)
    {
        EnsureSubmitted();

        Status = PurchaseRequestStatus.Approved;
        ReviewedByUserId = reviewerUserId;
        ReviewedAtUtc = reviewedAtUtc;
        ReviewRemarks = NormalizeOptional(remarks, 2000);
    }

    public void Reject(
        Guid reviewerUserId,
        DateTimeOffset reviewedAtUtc,
        string remarks)
    {
        EnsureSubmitted();

        if (string.IsNullOrWhiteSpace(remarks))
        {
            throw new DomainException(
                "Purchase request rejection remarks are required.");
        }

        Status = PurchaseRequestStatus.Rejected;
        ReviewedByUserId = reviewerUserId;
        ReviewedAtUtc = reviewedAtUtc;
        ReviewRemarks = NormalizeOptional(remarks, 2000);
    }

    public void MarkConverted()
    {
        if (Status != PurchaseRequestStatus.Approved)
        {
            throw new DomainException(
                "Only approved purchase requests can be converted.");
        }

        Status = PurchaseRequestStatus.Converted;
    }

    public void Cancel()
    {
        if (Status is PurchaseRequestStatus.Converted
            or PurchaseRequestStatus.Cancelled)
        {
            throw new DomainException(
                "Purchase request cannot be cancelled in its current status.");
        }

        Status = PurchaseRequestStatus.Cancelled;
    }

    private void ReplaceItems(IEnumerable<PurchaseRequestItemInput> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        PurchaseRequestItemInput[] values = [.. items];

        if (values.Length == 0)
        {
            throw new DomainException(
                "Purchase request must contain at least one item.");
        }

        _items.Clear();

        foreach (PurchaseRequestItemInput item in values)
        {
            _items.Add(new PurchaseRequestItem(
                Guid.CreateVersion7(),
                Id,
                item.Description,
                item.Quantity,
                item.Unit,
                item.EstimatedUnitCost));
        }
    }

    private void EnsureSubmitted()
    {
        if (Status != PurchaseRequestStatus.Submitted)
        {
            throw new DomainException(
                "Only submitted purchase requests can be reviewed.");
        }
    }

    private static void ValidateNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                "Purchase request number is required.");
        }

        if (value.Trim().Length > 100)
        {
            throw new DomainException(
                "Purchase request number cannot exceed 100 characters.");
        }
    }

    private static void ValidateTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                "Purchase request title is required.");
        }

        if (value.Trim().Length > 300)
        {
            throw new DomainException(
                "Purchase request title cannot exceed 300 characters.");
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

    private static string? NormalizeOptional(
        string? value,
        int maximumLength)
    {
        if (value?.Trim().Length > maximumLength)
        {
            throw new DomainException(
                $"Value cannot exceed {maximumLength} characters.");
        }

        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}

public sealed record PurchaseRequestItemInput(
    string Description,
    decimal Quantity,
    string Unit,
    decimal? EstimatedUnitCost);
