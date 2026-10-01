using Construction.Application.Common.Pagination;
using Construction.Domain.Notifications;

namespace Construction.Application.Abstractions.Data;

public interface INotificationRepository
{
    Task<Notification?> GetAsync(
        Guid notificationId,
        Guid recipientUserId,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Notification> Items, long TotalCount)> GetPageAsync(
        Guid recipientUserId,
        bool unreadOnly,
        PageRequest page,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Notification>> GetUnreadAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default);

    Task<NotificationPreference?> GetPreferenceAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    void Add(Notification notification);

    void AddPreference(NotificationPreference preference);
}
