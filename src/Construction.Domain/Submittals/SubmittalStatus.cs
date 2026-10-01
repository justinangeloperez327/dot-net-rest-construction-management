namespace Construction.Domain.Submittals;

public enum SubmittalStatus
{
    Draft = 0,
    Submitted = 1,
    UnderReview = 2,
    Approved = 3,
    ApprovedWithComments = 4,
    Rejected = 5,
    Closed = 6,
    Cancelled = 7
}
