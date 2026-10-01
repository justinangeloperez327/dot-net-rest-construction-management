using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(ApplicationDbContext dbContext)
    : INotificationRepository
{
    public Task<Notification?> GetAsync(
        Guid notificationId,
        Guid recipientUserId,
        CancellationToken cancellationToken = default) =>
        dbContext.Notifications.SingleOrDefaultAsync(
            notification =>
                notification.Id == notificationId
                && notification.RecipientUserId == recipientUserId,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Notification> Items, long TotalCount)> GetPageAsync(
        Guid recipientUserId,
        bool unreadOnly,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Notification> query = dbContext.Notifications
            .AsNoTracking()
            .Where(notification =>
                notification.RecipientUserId == recipientUserId);

        if (unreadOnly)
        {
            query = query.Where(notification => !notification.IsRead);
        }

        query = query.OrderByDescending(
            notification => notification.CreatedAtUtc);

        long totalCount = await query.LongCountAsync(cancellationToken);

        Notification[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IReadOnlyCollection<Notification>> GetUnreadAsync(
        Guid recipientUserId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Notifications
            .Where(notification =>
                notification.RecipientUserId == recipientUserId
                && !notification.IsRead)
            .ToArrayAsync(cancellationToken);

    public Task<NotificationPreference?> GetPreferenceAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        dbContext.NotificationPreferences.SingleOrDefaultAsync(
            preference => preference.UserId == userId,
            cancellationToken);

    public void Add(Notification notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        dbContext.Notifications.Add(notification);
    }

    public void AddPreference(NotificationPreference preference)
    {
        ArgumentNullException.ThrowIfNull(preference);
        dbContext.NotificationPreferences.Add(preference);
    }
}
