namespace Construction.Api.Contracts.DailyProgress;

public sealed record SetDailyProgressEquipmentRequest(
    IReadOnlyCollection<DailyProgressEquipmentRequestItem> Equipment);

public sealed record DailyProgressEquipmentRequestItem(
    string Description,
    int Quantity,
    decimal WorkingHours,
    decimal IdleHours);
