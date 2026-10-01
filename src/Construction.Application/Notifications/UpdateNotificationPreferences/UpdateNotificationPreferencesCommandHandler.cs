using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Notifications;

namespace Construction.Application.Notifications.UpdateNotificationPreferences;

public sealed class UpdateNotificationPreferencesCommandHandler(
    INotificationRepository notifications,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser)
    : ICommandHandler<UpdateNotificationPreferencesCommand, NotificationPreferenceResponse>
{
    public async Task<Result<NotificationPreferenceResponse>> HandleAsync(
        UpdateNotificationPreferencesCommand command,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<NotificationPreferenceResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        NotificationPreference? preference =
            await notifications.GetPreferenceAsync(
                userId,
                cancellationToken);

        if (preference is null)
        {
            preference = NotificationPreference.CreateDefault(userId);
            notifications.AddPreference(preference);
        }

        preference.Update(
            command.InAppEnabled,
            command.EmailEnabled);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            NotificationPreferenceResponse.FromDomain(preference));
    }
}
