namespace Construction.Api.Contracts.Notifications;

public sealed record UpdateNotificationPreferencesRequest(
    bool InAppEnabled,
    bool EmailEnabled);
