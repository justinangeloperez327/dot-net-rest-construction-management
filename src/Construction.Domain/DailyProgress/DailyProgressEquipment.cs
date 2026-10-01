using Construction.Domain.Common;

namespace Construction.Domain.DailyProgress;

public sealed class DailyProgressEquipment : AuditableEntity<Guid>
{
    private DailyProgressEquipment()
        : base(Guid.Empty)
    {
    }

    internal DailyProgressEquipment(
        Guid id,
        Guid reportId,
        string description,
        int quantity,
        decimal workingHours,
        decimal idleHours)
        : base(id)
    {
        ReportId = reportId;
        Update(description, quantity, workingHours, idleHours);
    }

    public Guid ReportId { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal WorkingHours { get; private set; }

    public decimal IdleHours { get; private set; }

    internal void Update(
        string description,
        int quantity,
        decimal workingHours,
        decimal idleHours)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(
                "Equipment description is required.");
        }

        if (description.Trim().Length > 200)
        {
            throw new DomainException(
                "Equipment description cannot exceed 200 characters.");
        }

        if (quantity <= 0)
        {
            throw new DomainException(
                "Equipment quantity must be greater than zero.");
        }

        if (workingHours < 0 || idleHours < 0)
        {
            throw new DomainException(
                "Equipment hours cannot be negative.");
        }

        Description = description.Trim();
        Quantity = quantity;
        WorkingHours = workingHours;
        IdleHours = idleHours;
    }
}
