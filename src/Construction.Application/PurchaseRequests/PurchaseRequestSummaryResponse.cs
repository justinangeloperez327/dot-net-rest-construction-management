using Construction.Domain.PurchaseRequests;

namespace Construction.Application.PurchaseRequests;

public sealed record PurchaseRequestSummaryResponse(
    Guid Id,
    string Number,
    string Title,
    string CurrencyCode,
    PurchaseRequestStatus Status,
    decimal EstimatedTotal,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static PurchaseRequestSummaryResponse FromDomain(PurchaseRequest request) =>
        new(
            request.Id,
            request.Number,
            request.Title,
            request.CurrencyCode,
            request.Status,
            request.EstimatedTotal,
            request.CreatedAtUtc,
            request.LastModifiedAtUtc);
}
