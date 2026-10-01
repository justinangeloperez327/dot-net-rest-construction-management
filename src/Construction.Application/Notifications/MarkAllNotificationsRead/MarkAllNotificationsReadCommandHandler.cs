using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Notifications.MarkAllNotificationsRead;

public sealed class MarkAllNotificationsReadCommandHandler(
    INotificationRepository notifications,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    TimeProvider timeProvider)
    : ICommandHandler<MarkAllNotificationsReadCommand>
{
    public async Task<Result> HandleAsync(
        MarkAllNotificationsReadCommand command,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var unread = await notifications.GetUnreadAsync(
            userId,
            cancellationToken);

        DateTimeOffset now = timeProvider.GetUtcNow();

        foreach (var notification in unread)
        {
            notification.MarkRead(now);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
