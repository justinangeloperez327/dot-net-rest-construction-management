namespace Construction.Domain.Submittals;

public enum SubmittalHistoryAction
{
    Created = 0,
    Updated = 1,
    RevisionAdded = 2,
    Submitted = 3,
    ReviewStarted = 4,
    Approved = 5,
    ApprovedWithComments = 6,
    Rejected = 7,
    Closed = 8,
    Cancelled = 9,
    CommentAdded = 10
}
