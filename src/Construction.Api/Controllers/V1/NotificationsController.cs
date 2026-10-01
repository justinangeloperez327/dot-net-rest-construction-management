using Construction.Api.Configuration;
using Construction.Api.Contracts.Notifications;
using Construction.Api.Extensions;
using Construction.Application.Common.Pagination;
using Construction.Application.Notifications.GetNotificationPreferences;
using Construction.Application.Notifications.GetNotifications;
using Construction.Application.Notifications.MarkAllNotificationsRead;
using Construction.Application.Notifications.MarkNotificationRead;
using Construction.Application.Notifications.UpdateNotificationPreferences;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/notifications")]
public sealed class NotificationsController(
    GetNotificationsQueryHandler listHandler,
    MarkNotificationReadCommandHandler markReadHandler,
    MarkAllNotificationsReadCommandHandler markAllReadHandler,
    GetNotificationPreferencesQueryHandler preferencesHandler,
    UpdateNotificationPreferencesCommandHandler updatePreferencesHandler)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListAsync(
        [FromQuery] bool unreadOnly = false,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetNotificationsQuery(
                unreadOnly,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{notificationId:guid}/read")]
    public async Task<IActionResult> MarkReadAsync(
        Guid notificationId,
        CancellationToken cancellationToken)
    {
        var result = await markReadHandler.HandleAsync(
            new MarkNotificationReadCommand(notificationId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllReadAsync(
        CancellationToken cancellationToken)
    {
        var result = await markAllReadHandler.HandleAsync(
            new MarkAllNotificationsReadCommand(),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetPreferencesAsync(
        CancellationToken cancellationToken)
    {
        var result = await preferencesHandler.HandleAsync(
            new GetNotificationPreferencesQuery(),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("preferences")]
    public async Task<IActionResult> UpdatePreferencesAsync(
        UpdateNotificationPreferencesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updatePreferencesHandler.HandleAsync(
            new UpdateNotificationPreferencesCommand(
                request.InAppEnabled,
                request.EmailEnabled),
            cancellationToken);

        return result.ToActionResult();
    }
}
