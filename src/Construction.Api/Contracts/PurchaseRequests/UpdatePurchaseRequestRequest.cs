namespace Construction.Api.Contracts.PurchaseRequests;

public sealed record UpdatePurchaseRequestRequest(
    string Title,
    string CurrencyCode,
    IReadOnlyCollection<PurchaseRequestItemRequest> Items);
