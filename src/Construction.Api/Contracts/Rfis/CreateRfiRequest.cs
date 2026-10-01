namespace Construction.Api.Contracts.Rfis;

public sealed record CreateRfiRequest(
    string Number,
    string Subject,
    string Question,
    DateOnly? DueDate,
    Guid? ResponsibleUserId);
