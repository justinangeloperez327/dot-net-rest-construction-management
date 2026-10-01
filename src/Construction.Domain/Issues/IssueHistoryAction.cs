namespace Construction.Domain.Issues;

public enum IssueHistoryAction
{
    Created = 0,
    Updated = 1,
    Started = 2,
    CorrectiveActionAdded = 3,
    CorrectiveActionUpdated = 4,
    SubmittedForVerification = 5,
    VerifiedClosed = 6,
    Reopened = 7,
    Cancelled = 8
}
