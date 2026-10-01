using Construction.Application.Abstractions.Messaging;
using Construction.Domain.Notifications;
using Construction.Infrastructure.Identity;
using Construction.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Construction.Infrastructure.Notifications;

public sealed class NotificationSender(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    IEmailSender emailSender,
    ILogger<NotificationSender> logger)
    : INotificationSender
{
    public async Task SendAsync(
        ApplicationNotification notification,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        ApplicationUser? user = await userManager.FindByIdAsync(
            notification.RecipientUserId.ToString());

        if (user is null || !user.IsActive)
        {
            logger.LogWarning(
                "Notification recipient {RecipientUserId} is unavailable.",
                notification.RecipientUserId);
            return;
        }

        NotificationPreference? preference =
            await dbContext.NotificationPreferences
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    value =>
                        value.UserId == notification.RecipientUserId,
                    cancellationToken);

        bool inAppEnabled = preference?.InAppEnabled ?? true;
        bool emailEnabled = preference?.EmailEnabled ?? false;

        if (inAppEnabled)
        {
            dbContext.Notifications.Add(
                Notification.Create(
                    notification.RecipientUserId,
                    notification.ProjectId,
                    notification.Type,
                    notification.Subject,
                    notification.Message,
                    notification.RelatedEntityType,
                    notification.RelatedEntityId));

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (emailEnabled
            && !string.IsNullOrWhiteSpace(user.Email))
        {
            await emailSender.SendAsync(
                new EmailMessage(
                    [user.Email],
                    notification.Subject,
                    notification.Message),
                cancellationToken);
        }
    }
}
