using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseRequests.CancelPurchaseRequest;

public sealed record CancelPurchaseRequestCommand(
    Guid ProjectId,
    Guid PurchaseRequestId) : ICommand<PurchaseRequestResponse>;
