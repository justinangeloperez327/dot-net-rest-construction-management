using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseRequests.SubmitPurchaseRequest;

public sealed record SubmitPurchaseRequestCommand(
    Guid ProjectId,
    Guid PurchaseRequestId) : ICommand<PurchaseRequestResponse>;
