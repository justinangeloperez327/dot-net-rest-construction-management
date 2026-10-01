using Construction.Domain.Notifications;

namespace Construction.Application.Notifications;

public sealed record NotificationResponse(
    Guid Id,
    Guid? ProjectId,
    string Type,
    string Subject,
    string Message,
    string? RelatedEntityType,
    Guid? RelatedEntityId,
    bool IsRead,
    DateTimeOffset? ReadAtUtc,
    DateTimeOffset CreatedAtUtc)
{
    public static NotificationResponse FromDomain(Notification notification) =>
        new(
            notification.Id,
            notification.ProjectId,
            notification.Type,
            notification.Subject,
            notification.Message,
            notification.RelatedEntityType,
            notification.RelatedEntityId,
            notification.IsRead,
            notification.ReadAtUtc,
            notification.CreatedAtUtc);
}
