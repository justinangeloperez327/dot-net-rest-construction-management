using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.DailyProgress;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.DailyProgress.CreateDailyProgress;
using Construction.Application.DailyProgress.DeleteDailyProgress;
using Construction.Application.DailyProgress.GetDailyProgress;
using Construction.Application.DailyProgress.GetDailyProgressReports;
using Construction.Application.DailyProgress.ReviewDailyProgress;
using Construction.Application.DailyProgress.SetDailyProgressActivities;
using Construction.Application.DailyProgress.SetDailyProgressEquipment;
using Construction.Application.DailyProgress.SetDailyProgressManpower;
using Construction.Application.DailyProgress.SubmitDailyProgress;
using Construction.Application.DailyProgress.UpdateDailyProgress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/daily-progress")]
public sealed class DailyProgressController(
    CreateDailyProgressCommandHandler createHandler,
    GetDailyProgressQueryHandler getHandler,
    GetDailyProgressReportsQueryHandler listHandler,
    UpdateDailyProgressCommandHandler updateHandler,
    SetDailyProgressActivitiesCommandHandler activitiesHandler,
    SetDailyProgressManpowerCommandHandler manpowerHandler,
    SetDailyProgressEquipmentCommandHandler equipmentHandler,
    SubmitDailyProgressCommandHandler submitHandler,
    ReviewDailyProgressCommandHandler reviewHandler,
    DeleteDailyProgressCommandHandler deleteHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.DailyProgress.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateDailyProgressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateDailyProgressCommand(
                projectId,
                request.ReportDate,
                request.Weather,
                request.TemperatureCelsius,
                request.WorkSummary,
                request.Remarks),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new
                {
                    projectId,
                    reportId = result.Value.Id
                },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{reportId:guid}")]
    [HasPermission(Permissions.DailyProgress.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetDailyProgressQuery(projectId, reportId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.DailyProgress.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetDailyProgressReportsQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{reportId:guid}")]
    [HasPermission(Permissions.DailyProgress.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid reportId,
        UpdateDailyProgressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateDailyProgressCommand(
                projectId,
                reportId,
                request.Weather,
                request.TemperatureCelsius,
                request.WorkSummary,
                request.Remarks),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{reportId:guid}/activities")]
    [HasPermission(Permissions.DailyProgress.Manage)]
    public async Task<IActionResult> SetActivitiesAsync(
        Guid projectId,
        Guid reportId,
        SetDailyProgressActivitiesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await activitiesHandler.HandleAsync(
            new SetDailyProgressActivitiesCommand(
                projectId,
                reportId,
                request.Activities
                    .Select(item => new DailyProgressActivityEntry(
                        item.ActivityId,
                        item.WorkDescription,
                        item.ReportedProgressPercentage,
                        item.QuantityCompleted,
                        item.Unit))
                    .ToArray()),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{reportId:guid}/manpower")]
    [HasPermission(Permissions.DailyProgress.Manage)]
    public async Task<IActionResult> SetManpowerAsync(
        Guid projectId,
        Guid reportId,
        SetDailyProgressManpowerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await manpowerHandler.HandleAsync(
            new SetDailyProgressManpowerCommand(
                projectId,
                reportId,
                request.Manpower
                    .Select(item => new DailyProgressManpowerEntry(
                        item.CompanyId,
                        item.Trade,
                        item.Headcount,
                        item.TotalHours))
                    .ToArray()),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{reportId:guid}/equipment")]
    [HasPermission(Permissions.DailyProgress.Manage)]
    public async Task<IActionResult> SetEquipmentAsync(
        Guid projectId,
        Guid reportId,
        SetDailyProgressEquipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await equipmentHandler.HandleAsync(
            new SetDailyProgressEquipmentCommand(
                projectId,
                reportId,
                request.Equipment
                    .Select(item => new DailyProgressEquipmentEntry(
                        item.Description,
                        item.Quantity,
                        item.WorkingHours,
                        item.IdleHours))
                    .ToArray()),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{reportId:guid}/submit")]
    [HasPermission(Permissions.DailyProgress.Manage)]
    public async Task<IActionResult> SubmitAsync(
        Guid projectId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var result = await submitHandler.HandleAsync(
            new SubmitDailyProgressCommand(projectId, reportId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{reportId:guid}/review")]
    [HasPermission(Permissions.DailyProgress.Approve)]
    public async Task<IActionResult> ReviewAsync(
        Guid projectId,
        Guid reportId,
        ReviewDailyProgressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await reviewHandler.HandleAsync(
            new ReviewDailyProgressCommand(
                projectId,
                reportId,
                request.Approve,
                request.RejectionReason),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{reportId:guid}")]
    [HasPermission(Permissions.DailyProgress.Manage)]
    public async Task<IActionResult> DeleteAsync(
        Guid projectId,
        Guid reportId,
        CancellationToken cancellationToken)
    {
        var result = await deleteHandler.HandleAsync(
            new DeleteDailyProgressCommand(projectId, reportId),
            cancellationToken);

        return result.ToActionResult();
    }
}
