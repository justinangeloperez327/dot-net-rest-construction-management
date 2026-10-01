namespace Construction.Domain.PurchaseRequests;

public enum PurchaseRequestStatus
{
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    Rejected = 3,
    Converted = 4,
    Cancelled = 5
}
