using Construction.Domain.Equipment;

namespace Construction.Application.Equipment;

public sealed record EquipmentSummaryResponse(
    Guid Id,
    string AssetCode,
    string Name,
    string? Make,
    string? Model,
    string? SerialNumber,
    EquipmentStatus Status,
    DateTimeOffset CreatedAtUtc)
{
    public static EquipmentSummaryResponse FromDomain(
        Construction.Domain.Equipment.Equipment equipment) =>
        new(
            equipment.Id,
            equipment.AssetCode,
            equipment.Name,
            equipment.Make,
            equipment.Model,
            equipment.SerialNumber,
            equipment.Status,
            equipment.CreatedAtUtc);
}
