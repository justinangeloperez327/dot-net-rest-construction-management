using Construction.Application.Common.Messaging;

namespace Construction.Application.Equipment.GetEquipment;

public sealed record GetEquipmentQuery(
    Guid ProjectId,
    Guid EquipmentId) : IQuery<EquipmentResponse>;
