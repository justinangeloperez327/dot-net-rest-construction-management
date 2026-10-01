using Construction.Application.Common.Messaging;

namespace Construction.Application.Notifications.UpdateNotificationPreferences;

public sealed record UpdateNotificationPreferencesCommand(
    bool InAppEnabled,
    bool EmailEnabled)
    : ICommand<NotificationPreferenceResponse>;
