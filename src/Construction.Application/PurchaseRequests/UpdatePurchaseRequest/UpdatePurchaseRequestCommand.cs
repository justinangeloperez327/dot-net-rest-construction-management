using Construction.Application.Common.Messaging;
using Construction.Application.PurchaseRequests.CreatePurchaseRequest;

namespace Construction.Application.PurchaseRequests.UpdatePurchaseRequest;

public sealed record UpdatePurchaseRequestCommand(
    Guid ProjectId,
    Guid PurchaseRequestId,
    string Title,
    string CurrencyCode,
    IReadOnlyCollection<PurchaseRequestItemInputModel> Items)
    : ICommand<PurchaseRequestResponse>;
