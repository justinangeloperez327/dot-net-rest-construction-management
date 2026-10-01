using Construction.Application.Common.Messaging;

namespace Construction.Application.Equipment.ReturnEquipment;

public sealed record ReturnEquipmentCommand(
    Guid ProjectId,
    Guid EquipmentId) : ICommand<EquipmentResponse>;
