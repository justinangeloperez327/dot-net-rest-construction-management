using Construction.Application.Common.Messaging;

namespace Construction.Application.Equipment.Maintenance;

public sealed record CompleteEquipmentMaintenanceCommand(
    Guid ProjectId,
    Guid EquipmentId,
    Guid MaintenanceRecordId,
    DateOnly CompletedDate,
    decimal? Cost,
    string? CompletionNotes) : ICommand<EquipmentResponse>;
