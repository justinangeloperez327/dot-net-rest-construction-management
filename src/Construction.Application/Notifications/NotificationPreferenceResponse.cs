using Construction.Domain.Notifications;

namespace Construction.Application.Notifications;

public sealed record NotificationPreferenceResponse(
    bool InAppEnabled,
    bool EmailEnabled)
{
    public static NotificationPreferenceResponse FromDomain(
        NotificationPreference preference) =>
        new(
            preference.InAppEnabled,
            preference.EmailEnabled);
}
