using Construction.Domain.PurchaseRequests;

namespace Construction.Application.PurchaseRequests;

public sealed record PurchaseRequestResponse(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Title,
    string CurrencyCode,
    Guid RequestedByUserId,
    PurchaseRequestStatus Status,
    Guid? ReviewedByUserId,
    DateTimeOffset? ReviewedAtUtc,
    string? ReviewRemarks,
    decimal EstimatedTotal,
    IReadOnlyCollection<PurchaseRequestItemResponse> Items,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static PurchaseRequestResponse FromDomain(PurchaseRequest request) =>
        new(
            request.Id,
            request.ProjectId,
            request.Number,
            request.Title,
            request.CurrencyCode,
            request.RequestedByUserId,
            request.Status,
            request.ReviewedByUserId,
            request.ReviewedAtUtc,
            request.ReviewRemarks,
            request.EstimatedTotal,
            request.Items.Select(PurchaseRequestItemResponse.FromDomain).ToArray(),
            request.CreatedAtUtc,
            request.LastModifiedAtUtc);
}

public sealed record PurchaseRequestItemResponse(
    Guid Id,
    string Description,
    decimal Quantity,
    string Unit,
    decimal? EstimatedUnitCost)
{
    public static PurchaseRequestItemResponse FromDomain(PurchaseRequestItem item) =>
        new(item.Id, item.Description, item.Quantity, item.Unit, item.EstimatedUnitCost);
}
