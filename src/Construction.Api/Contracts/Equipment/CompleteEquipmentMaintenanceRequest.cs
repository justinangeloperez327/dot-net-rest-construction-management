namespace Construction.Api.Contracts.Equipment;

public sealed record CompleteEquipmentMaintenanceRequest(
    DateOnly CompletedDate,
    decimal? Cost,
    string? CompletionNotes);
