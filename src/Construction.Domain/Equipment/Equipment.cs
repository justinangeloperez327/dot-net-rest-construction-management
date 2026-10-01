using Construction.Domain.Common;

namespace Construction.Domain.Equipment;

public sealed class Equipment : AuditableAggregateRoot<Guid>
{
    private readonly List<EquipmentAssignment> _assignments = [];
    private readonly List<EquipmentMaintenanceRecord> _maintenanceRecords = [];

    private Equipment()
        : base(Guid.Empty)
    {
    }

    private Equipment(
        Guid id,
        Guid projectId,
        string assetCode,
        string name,
        string? make,
        string? model,
        string? serialNumber)
        : base(id)
    {
        ProjectId = projectId;
        AssetCode = assetCode;
        NormalizedAssetCode = Normalize(assetCode);
        Name = name;
        Make = NormalizeOptional(make, 100);
        Model = NormalizeOptional(model, 100);
        SerialNumber = NormalizeOptional(serialNumber, 150);
        Status = EquipmentStatus.Available;
    }

    public Guid ProjectId { get; private set; }
    public string AssetCode { get; private set; } = string.Empty;
    public string NormalizedAssetCode { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Make { get; private set; }
    public string? Model { get; private set; }
    public string? SerialNumber { get; private set; }
    public EquipmentStatus Status { get; private set; }

    public IReadOnlyCollection<EquipmentAssignment> Assignments => _assignments;
    public IReadOnlyCollection<EquipmentMaintenanceRecord> MaintenanceRecords => _maintenanceRecords;

    public static Equipment Create(
        Guid projectId,
        string assetCode,
        string name,
        string? make,
        string? model,
        string? serialNumber)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException("Project identifier is required.");
        }

        ValidateAssetCode(assetCode);
        ValidateName(name);

        return new Equipment(
            Guid.CreateVersion7(),
            projectId,
            assetCode.Trim(),
            name.Trim(),
            make,
            model,
            serialNumber);
    }

    public void Update(
        string name,
        string? make,
        string? model,
        string? serialNumber)
    {
        if (Status == EquipmentStatus.Retired)
        {
            throw new DomainException(
                "Retired equipment cannot be updated.");
        }

        ValidateName(name);

        Name = name.Trim();
        Make = NormalizeOptional(make, 100);
        Model = NormalizeOptional(model, 100);
        SerialNumber = NormalizeOptional(serialNumber, 150);
    }

    public EquipmentAssignment Assign(
        Guid? userId,
        Guid? locationId,
        DateTimeOffset assignedAtUtc,
        string? notes)
    {
        if (Status is EquipmentStatus.Maintenance
            or EquipmentStatus.OutOfService
            or EquipmentStatus.Retired)
        {
            throw new DomainException(
                "Equipment cannot be assigned in its current status.");
        }

        if (userId is null && locationId is null)
        {
            throw new DomainException(
                "Equipment assignment requires a user or location.");
        }

        if (_assignments.Any(assignment => assignment.IsActive))
        {
            throw new DomainException(
                "Equipment already has an active assignment.");
        }

        var assignment = new EquipmentAssignment(
            Guid.CreateVersion7(),
            Id,
            userId,
            locationId,
            assignedAtUtc,
            notes);

        _assignments.Add(assignment);
        Status = EquipmentStatus.InUse;

        return assignment;
    }

    public void Return(DateTimeOffset returnedAtUtc)
    {
        EquipmentAssignment assignment =
            _assignments.SingleOrDefault(item => item.IsActive)
            ?? throw new DomainException(
                "Equipment does not have an active assignment.");

        assignment.Return(returnedAtUtc);
        Status = EquipmentStatus.Available;
    }

    public EquipmentMaintenanceRecord ScheduleMaintenance(
        string description,
        DateOnly scheduledDate,
        string? serviceProvider)
    {
        if (Status == EquipmentStatus.Retired)
        {
            throw new DomainException(
                "Maintenance cannot be scheduled for retired equipment.");
        }

        if (_maintenanceRecords.Any(record => !record.IsCompleted))
        {
            throw new DomainException(
                "Equipment already has an open maintenance record.");
        }

        if (_assignments.Any(assignment => assignment.IsActive))
        {
            throw new DomainException(
                "Assigned equipment must be returned before maintenance.");
        }

        var record = new EquipmentMaintenanceRecord(
            Guid.CreateVersion7(),
            Id,
            description,
            scheduledDate,
            serviceProvider);

        _maintenanceRecords.Add(record);
        Status = EquipmentStatus.Maintenance;

        return record;
    }

    public void CompleteMaintenance(
        Guid maintenanceRecordId,
        DateOnly completedDate,
        decimal? cost,
        string? completionNotes)
    {
        EquipmentMaintenanceRecord record =
            _maintenanceRecords.SingleOrDefault(item =>
                item.Id == maintenanceRecordId)
            ?? throw new DomainException(
                "Equipment maintenance record was not found.");

        record.Complete(completedDate, cost, completionNotes);
        Status = EquipmentStatus.Available;
    }

    public void MarkOutOfService()
    {
        if (_assignments.Any(assignment => assignment.IsActive))
        {
            throw new DomainException(
                "Assigned equipment must be returned first.");
        }

        if (Status == EquipmentStatus.Retired)
        {
            throw new DomainException(
                "Retired equipment cannot change status.");
        }

        Status = EquipmentStatus.OutOfService;
    }

    public void RestoreToAvailable()
    {
        if (Status != EquipmentStatus.OutOfService)
        {
            throw new DomainException(
                "Only out-of-service equipment can be restored.");
        }

        Status = EquipmentStatus.Available;
    }

    public void Retire()
    {
        if (_assignments.Any(assignment => assignment.IsActive))
        {
            throw new DomainException(
                "Assigned equipment must be returned before retirement.");
        }

        Status = EquipmentStatus.Retired;
    }

    private static void ValidateAssetCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Equipment asset code is required.");
        }

        if (value.Trim().Length > 100)
        {
            throw new DomainException(
                "Equipment asset code cannot exceed 100 characters.");
        }
    }

    private static void ValidateName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Equipment name is required.");
        }

        if (value.Trim().Length > 200)
        {
            throw new DomainException(
                "Equipment name cannot exceed 200 characters.");
        }
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
