namespace Construction.Api.Contracts.PurchaseRequests;

public sealed record ReviewPurchaseRequestRequest(
    bool Approve,
    string? Remarks);
