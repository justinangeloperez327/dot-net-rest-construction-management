namespace Construction.Api.Contracts.Issues;

public sealed record AddCorrectiveActionRequest(
    string Description,
    Guid? ResponsibleUserId,
    DateOnly? DueDate);
