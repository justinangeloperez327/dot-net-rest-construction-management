using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseRequests.GetPurchaseRequest;

public sealed record GetPurchaseRequestQuery(
    Guid ProjectId,
    Guid PurchaseRequestId) : IQuery<PurchaseRequestResponse>;
