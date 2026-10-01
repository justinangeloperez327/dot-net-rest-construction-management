namespace Construction.Api.Contracts.Equipment;

public sealed record CreateEquipmentRequest(
    string AssetCode,
    string Name,
    string? Make,
    string? Model,
    string? SerialNumber);
