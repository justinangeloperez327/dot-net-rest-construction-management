using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Reports.GetActivityReport;
using Construction.Application.Reports.GetDailyProgressReport;
using Construction.Application.Reports.GetProcurementReport;
using Construction.Application.Reports.GetProjectSummary;
using Construction.Application.Reports.GetQualityReport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/reports")]
public sealed class ReportsController(
    GetProjectSummaryQueryHandler summaryHandler,
    GetActivityReportQueryHandler activityHandler,
    GetDailyProgressReportQueryHandler dailyProgressHandler,
    GetQualityReportQueryHandler qualityHandler,
    GetProcurementReportQueryHandler procurementHandler)
    : ControllerBase
{
    [HttpGet("summary")]
    [HasPermission(Permissions.Reports.View)]
    public async Task<IActionResult> GetSummaryAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await summaryHandler.HandleAsync(
            new GetProjectSummaryQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("activities")]
    [HasPermission(Permissions.Reports.View)]
    public async Task<IActionResult> GetActivitiesAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await activityHandler.HandleAsync(
            new GetActivityReportQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("daily-progress")]
    [HasPermission(Permissions.Reports.View)]
    public async Task<IActionResult> GetDailyProgressAsync(
        Guid projectId,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var result = await dailyProgressHandler.HandleAsync(
            new GetDailyProgressReportQuery(
                projectId,
                fromDate,
                toDate),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("quality")]
    [HasPermission(Permissions.Reports.View)]
    public async Task<IActionResult> GetQualityAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await qualityHandler.HandleAsync(
            new GetQualityReportQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet("procurement")]
    [HasPermission(Permissions.Reports.View)]
    public async Task<IActionResult> GetProcurementAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await procurementHandler.HandleAsync(
            new GetProcurementReportQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }
}
