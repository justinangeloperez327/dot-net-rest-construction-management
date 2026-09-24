namespace Construction.Application.Abstractions.Messaging;

public interface INotificationSender
{
    Task SendAsync(
        ApplicationNotification notification,
        CancellationToken cancellationToken = default);
}
