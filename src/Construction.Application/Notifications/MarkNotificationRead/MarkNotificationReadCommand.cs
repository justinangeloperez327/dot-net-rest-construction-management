using Construction.Application.Common.Messaging;

namespace Construction.Application.Notifications.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(
    Guid NotificationId) : ICommand<NotificationResponse>;
