using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Notifications;

namespace Construction.Application.Notifications.GetNotificationPreferences;

public sealed class GetNotificationPreferencesQueryHandler(
    INotificationRepository notifications,
    ICurrentUser currentUser)
    : IQueryHandler<GetNotificationPreferencesQuery, NotificationPreferenceResponse>
{
    public async Task<Result<NotificationPreferenceResponse>> HandleAsync(
        GetNotificationPreferencesQuery query,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<NotificationPreferenceResponse>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        NotificationPreference preference =
            await notifications.GetPreferenceAsync(
                userId,
                cancellationToken)
            ?? NotificationPreference.CreateDefault(userId);

        return Result.Success(
            NotificationPreferenceResponse.FromDomain(preference));
    }
}
