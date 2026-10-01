namespace Construction.Api.Contracts.PurchaseRequests;

public sealed record CreatePurchaseRequestRequest(
    string Number,
    string Title,
    string CurrencyCode,
    IReadOnlyCollection<PurchaseRequestItemRequest> Items);

public sealed record PurchaseRequestItemRequest(
    string Description,
    decimal Quantity,
    string Unit,
    decimal? EstimatedUnitCost);
