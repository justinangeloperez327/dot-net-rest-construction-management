using Construction.Application.Common.Messaging;

namespace Construction.Application.Equipment.CreateEquipment;

public sealed record CreateEquipmentCommand(
    Guid ProjectId,
    string AssetCode,
    string Name,
    string? Make,
    string? Model,
    string? SerialNumber) : ICommand<EquipmentResponse>;
