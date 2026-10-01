using Construction.Domain.Equipment;

namespace Construction.Api.Contracts.Equipment;

public sealed record ChangeEquipmentStatusRequest(EquipmentStatus Status);
