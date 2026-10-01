using Construction.Application.Common.Messaging;

namespace Construction.Application.PurchaseRequests.CreatePurchaseRequest;

public sealed record CreatePurchaseRequestCommand(
    Guid ProjectId,
    string Number,
    string Title,
    string CurrencyCode,
    IReadOnlyCollection<PurchaseRequestItemInputModel> Items)
    : ICommand<PurchaseRequestResponse>;

public sealed record PurchaseRequestItemInputModel(
    string Description,
    decimal Quantity,
    string Unit,
    decimal? EstimatedUnitCost);
