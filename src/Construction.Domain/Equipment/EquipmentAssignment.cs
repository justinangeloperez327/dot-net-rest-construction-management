using Construction.Domain.Common;

namespace Construction.Domain.Equipment;

public sealed class EquipmentAssignment : AuditableEntity<Guid>
{
    private EquipmentAssignment()
        : base(Guid.Empty)
    {
    }

    internal EquipmentAssignment(
        Guid id,
        Guid equipmentId,
        Guid? userId,
        Guid? locationId,
        DateTimeOffset assignedAtUtc,
        string? notes)
        : base(id)
    {
        EquipmentId = equipmentId;
        UserId = userId;
        LocationId = locationId;
        AssignedAtUtc = assignedAtUtc;
        Notes = NormalizeOptional(notes);
    }

    public Guid EquipmentId { get; private set; }
    public Guid? UserId { get; private set; }
    public Guid? LocationId { get; private set; }
    public DateTimeOffset AssignedAtUtc { get; private set; }
    public DateTimeOffset? ReturnedAtUtc { get; private set; }
    public string? Notes { get; private set; }
    public bool IsActive => ReturnedAtUtc is null;

    internal void Return(DateTimeOffset returnedAtUtc)
    {
        if (ReturnedAtUtc is not null)
        {
            throw new DomainException("Equipment assignment is already closed.");
        }

        if (returnedAtUtc < AssignedAtUtc)
        {
            throw new DomainException(
                "Equipment return time cannot precede assignment time.");
        }

        ReturnedAtUtc = returnedAtUtc;
    }

    private static string? NormalizeOptional(string? value)
    {
        if (value?.Trim().Length > 2000)
        {
            throw new DomainException(
                "Equipment assignment notes cannot exceed 2000 characters.");
        }

        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
