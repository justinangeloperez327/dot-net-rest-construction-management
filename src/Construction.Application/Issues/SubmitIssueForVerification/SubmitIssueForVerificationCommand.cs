using Construction.Application.Common.Messaging;

namespace Construction.Application.Issues.SubmitIssueForVerification;

public sealed record SubmitIssueForVerificationCommand(
    Guid ProjectId,
    Guid IssueId,
    string ResolutionSummary) : ICommand<IssueResponse>;
