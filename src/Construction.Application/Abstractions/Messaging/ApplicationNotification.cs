namespace Construction.Application.Abstractions.Messaging;

public sealed record ApplicationNotification(
    Guid RecipientUserId,
    string Type,
    string Subject,
    string Message);
