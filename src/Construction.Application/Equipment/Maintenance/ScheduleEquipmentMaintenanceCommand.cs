using Construction.Application.Common.Messaging;

namespace Construction.Application.Equipment.Maintenance;

public sealed record ScheduleEquipmentMaintenanceCommand(
    Guid ProjectId,
    Guid EquipmentId,
    string Description,
    DateOnly ScheduledDate,
    string? ServiceProvider) : ICommand<EquipmentMaintenanceResponse>;
