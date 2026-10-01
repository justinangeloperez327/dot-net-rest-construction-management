using Construction.Domain.Common;

namespace Construction.Domain.Equipment;

public sealed class EquipmentMaintenanceRecord : AuditableEntity<Guid>
{
    private EquipmentMaintenanceRecord()
        : base(Guid.Empty)
    {
    }

    internal EquipmentMaintenanceRecord(
        Guid id,
        Guid equipmentId,
        string description,
        DateOnly scheduledDate,
        string? serviceProvider)
        : base(id)
    {
        EquipmentId = equipmentId;
        Description = ValidateDescription(description);
        ScheduledDate = scheduledDate;
        ServiceProvider = NormalizeOptional(serviceProvider, 200);
    }

    public Guid EquipmentId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public DateOnly ScheduledDate { get; private set; }
    public string? ServiceProvider { get; private set; }
    public DateOnly? CompletedDate { get; private set; }
    public decimal? Cost { get; private set; }
    public string? CompletionNotes { get; private set; }
    public bool IsCompleted => CompletedDate is not null;

    internal void Complete(
        DateOnly completedDate,
        decimal? cost,
        string? completionNotes)
    {
        if (IsCompleted)
        {
            throw new DomainException(
                "Equipment maintenance record is already completed.");
        }

        if (completedDate < ScheduledDate)
        {
            throw new DomainException(
                "Maintenance completion date cannot precede the scheduled date.");
        }

        if (cost < 0)
        {
            throw new DomainException(
                "Maintenance cost cannot be negative.");
        }

        CompletedDate = completedDate;
        Cost = cost;
        CompletionNotes = NormalizeOptional(completionNotes, 4000);
    }

    private static string ValidateDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                "Maintenance description is required.");
        }

        if (value.Trim().Length > 2000)
        {
            throw new DomainException(
                "Maintenance description cannot exceed 2000 characters.");
        }

        return value.Trim();
    }

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
