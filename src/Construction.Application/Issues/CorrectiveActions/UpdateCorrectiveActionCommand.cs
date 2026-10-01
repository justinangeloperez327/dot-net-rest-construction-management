using Construction.Application.Common.Messaging;
using Construction.Domain.Issues;

namespace Construction.Application.Issues.CorrectiveActions;

public sealed record UpdateCorrectiveActionCommand(
    Guid ProjectId,
    Guid IssueId,
    Guid CorrectiveActionId,
    string Description,
    Guid? ResponsibleUserId,
    DateOnly? DueDate,
    CorrectiveActionStatus Status,
    string? CompletionNotes) : ICommand<CorrectiveActionResponse>;
