namespace Construction.Api.Contracts.Equipment;

public sealed record ScheduleEquipmentMaintenanceRequest(
    string Description,
    DateOnly ScheduledDate,
    string? ServiceProvider);
