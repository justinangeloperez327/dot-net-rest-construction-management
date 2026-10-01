namespace Construction.Api.Contracts.Equipment;

public sealed record UpdateEquipmentRequest(
    string Name,
    string? Make,
    string? Model,
    string? SerialNumber);
