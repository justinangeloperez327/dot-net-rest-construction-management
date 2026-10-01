using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Notifications.GetNotifications;

public sealed class GetNotificationsQueryHandler(
    INotificationRepository notifications,
    ICurrentUser currentUser)
    : IQueryHandler<GetNotificationsQuery, PagedResult<NotificationResponse>>
{
    public async Task<Result<PagedResult<NotificationResponse>>> HandleAsync(
        GetNotificationsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PagedResult<NotificationResponse>>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        var page = await notifications.GetPageAsync(
            userId,
            query.UnreadOnly,
            query.Page,
            cancellationToken);

        return Result.Success(
            new PagedResult<NotificationResponse>(
                page.Items.Select(NotificationResponse.FromDomain).ToArray(),
                Math.Max(1, query.Page.PageNumber),
                Math.Clamp(
                    query.Page.PageSize,
                    1,
                    PageRequest.MaximumPageSize),
                page.TotalCount));
    }
}
