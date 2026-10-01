using Construction.Domain.Issues;

namespace Construction.Api.Contracts.Issues;

public sealed record UpdateCorrectiveActionRequest(
    string Description,
    Guid? ResponsibleUserId,
    DateOnly? DueDate,
    CorrectiveActionStatus Status,
    string? CompletionNotes);
