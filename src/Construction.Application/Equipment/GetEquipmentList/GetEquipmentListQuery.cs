using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Equipment.GetEquipmentList;

public sealed record GetEquipmentListQuery(
    Guid ProjectId,
    PageRequest Page) : IQuery<PagedResult<EquipmentSummaryResponse>>;
