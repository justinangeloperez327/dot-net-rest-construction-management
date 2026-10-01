using Construction.Application.Common.Messaging;
using Construction.Domain.Equipment;

namespace Construction.Application.Equipment.ChangeEquipmentStatus;

public sealed record ChangeEquipmentStatusCommand(
    Guid ProjectId,
    Guid EquipmentId,
    EquipmentStatus Status) : ICommand<EquipmentResponse>;
