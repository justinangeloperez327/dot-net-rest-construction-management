using Construction.Application.Common.Messaging;

namespace Construction.Application.Equipment.AssignEquipment;

public sealed record AssignEquipmentCommand(
    Guid ProjectId,
    Guid EquipmentId,
    Guid? UserId,
    Guid? LocationId,
    string? Notes) : ICommand<EquipmentAssignmentResponse>;
