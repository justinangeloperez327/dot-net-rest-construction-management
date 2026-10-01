using Construction.Application.Common.Messaging;

namespace Construction.Application.Equipment.UpdateEquipment;

public sealed record UpdateEquipmentCommand(
    Guid ProjectId,
    Guid EquipmentId,
    string Name,
    string? Make,
    string? Model,
    string? SerialNumber) : ICommand<EquipmentResponse>;
