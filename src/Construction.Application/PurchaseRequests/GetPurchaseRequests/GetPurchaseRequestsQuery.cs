using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.PurchaseRequests.GetPurchaseRequests;

public sealed record GetPurchaseRequestsQuery(
    Guid ProjectId,
    PageRequest Page) : IQuery<PagedResult<PurchaseRequestSummaryResponse>>;
