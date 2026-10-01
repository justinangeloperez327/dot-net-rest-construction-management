using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;

namespace Construction.Application.Notifications.GetNotifications;

public sealed record GetNotificationsQuery(
    bool UnreadOnly,
    PageRequest Page)
    : IQuery<PagedResult<NotificationResponse>>;
