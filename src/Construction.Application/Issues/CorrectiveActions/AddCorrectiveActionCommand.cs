using Construction.Application.Common.Messaging;

namespace Construction.Application.Issues.CorrectiveActions;

public sealed record AddCorrectiveActionCommand(
    Guid ProjectId,
    Guid IssueId,
    string Description,
    Guid? ResponsibleUserId,
    DateOnly? DueDate) : ICommand<CorrectiveActionResponse>;
