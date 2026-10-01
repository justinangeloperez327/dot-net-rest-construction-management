namespace Construction.Api.Contracts.Equipment;

public sealed record AssignEquipmentRequest(
    Guid? UserId,
    Guid? LocationId,
    string? Notes);
