using Construction.Application.Common.Messaging;

namespace Construction.Application.Notifications.GetNotificationPreferences;

public sealed record GetNotificationPreferencesQuery
    : IQuery<NotificationPreferenceResponse>;
