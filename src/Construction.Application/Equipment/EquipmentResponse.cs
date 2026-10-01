using Construction.Domain.Equipment;

namespace Construction.Application.Equipment;

public sealed record EquipmentResponse(
    Guid Id,
    Guid ProjectId,
    string AssetCode,
    string Name,
    string? Make,
    string? Model,
    string? SerialNumber,
    EquipmentStatus Status,
    IReadOnlyCollection<EquipmentAssignmentResponse> Assignments,
    IReadOnlyCollection<EquipmentMaintenanceResponse> MaintenanceRecords,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static EquipmentResponse FromDomain(
        Construction.Domain.Equipment.Equipment equipment) =>
        new(
            equipment.Id,
            equipment.ProjectId,
            equipment.AssetCode,
            equipment.Name,
            equipment.Make,
            equipment.Model,
            equipment.SerialNumber,
            equipment.Status,
            equipment.Assignments
                .OrderByDescending(item => item.AssignedAtUtc)
                .Select(EquipmentAssignmentResponse.FromDomain)
                .ToArray(),
            equipment.MaintenanceRecords
                .OrderByDescending(item => item.ScheduledDate)
                .Select(EquipmentMaintenanceResponse.FromDomain)
                .ToArray(),
            equipment.CreatedAtUtc,
            equipment.LastModifiedAtUtc);
}

public sealed record EquipmentAssignmentResponse(
    Guid Id,
    Guid? UserId,
    Guid? LocationId,
    DateTimeOffset AssignedAtUtc,
    DateTimeOffset? ReturnedAtUtc,
    string? Notes,
    bool IsActive)
{
    public static EquipmentAssignmentResponse FromDomain(
        EquipmentAssignment assignment) =>
        new(
            assignment.Id,
            assignment.UserId,
            assignment.LocationId,
            assignment.AssignedAtUtc,
            assignment.ReturnedAtUtc,
            assignment.Notes,
            assignment.IsActive);
}

public sealed record EquipmentMaintenanceResponse(
    Guid Id,
    string Description,
    DateOnly ScheduledDate,
    string? ServiceProvider,
    DateOnly? CompletedDate,
    decimal? Cost,
    string? CompletionNotes,
    bool IsCompleted)
{
    public static EquipmentMaintenanceResponse FromDomain(
        EquipmentMaintenanceRecord record) =>
        new(
            record.Id,
            record.Description,
            record.ScheduledDate,
            record.ServiceProvider,
            record.CompletedDate,
            record.Cost,
            record.CompletionNotes,
            record.IsCompleted);
}
