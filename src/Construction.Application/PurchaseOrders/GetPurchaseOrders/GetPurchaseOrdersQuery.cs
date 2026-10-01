using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.PurchaseOrders.GetPurchaseOrders;

public sealed record GetPurchaseOrdersQuery(
    Guid ProjectId,
    PageRequest Page) : IQuery<PagedResult<PurchaseOrderSummaryResponse>>;
