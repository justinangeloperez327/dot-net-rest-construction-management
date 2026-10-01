using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Notifications.MarkNotificationRead;

public sealed class MarkNotificationReadCommandHandler(
    INotificationRepository notifications,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    TimeProvider timeProvider)
    : ICommandHandler<MarkNotificationReadCommand, NotificationResponse>
{
    public async Task<Result<NotificationResponse>> HandleAsync(
        MarkNotificationReadCommand command,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<NotificationResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var notification = await notifications.GetAsync(
            command.NotificationId,
            userId,
            cancellationToken);

        if (notification is null)
        {
            return Result.Failure<NotificationResponse>(
                ApplicationError.NotFound(
                    "Notifications.NotFound",
                    "The notification was not found."));
        }

        notification.MarkRead(timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            NotificationResponse.FromDomain(notification));
    }
}
