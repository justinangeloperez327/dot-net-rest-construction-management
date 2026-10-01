using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseRequests.ReviewPurchaseRequest;

public sealed record ReviewPurchaseRequestCommand(
    Guid ProjectId,
    Guid PurchaseRequestId,
    bool Approve,
    string? Remarks) : ICommand<PurchaseRequestResponse>;
