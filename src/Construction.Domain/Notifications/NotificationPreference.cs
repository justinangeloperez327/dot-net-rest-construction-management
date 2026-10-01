using Construction.Domain.Common;

namespace Construction.Domain.Notifications;

public sealed class NotificationPreference : AuditableAggregateRoot<Guid>
{
    private NotificationPreference()
        : base(Guid.Empty)
    {
    }

    private NotificationPreference(
        Guid id,
        Guid userId)
        : base(id)
    {
        UserId = userId;
        InAppEnabled = true;
        EmailEnabled = false;
    }

    public Guid UserId { get; private set; }

    public bool InAppEnabled { get; private set; }

    public bool EmailEnabled { get; private set; }

    public static NotificationPreference CreateDefault(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            throw new DomainException(
                "Notification preference user identifier is required.");
        }

        return new NotificationPreference(
            Guid.CreateVersion7(),
            userId);
    }

    public void Update(
        bool inAppEnabled,
        bool emailEnabled)
    {
        InAppEnabled = inAppEnabled;
        EmailEnabled = emailEnabled;
    }
}
