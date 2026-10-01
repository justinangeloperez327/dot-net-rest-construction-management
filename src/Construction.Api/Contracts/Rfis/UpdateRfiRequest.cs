namespace Construction.Api.Contracts.Rfis;

public sealed record UpdateRfiRequest(
    string Subject,
    string Question,
    DateOnly? DueDate,
    Guid? ResponsibleUserId);
