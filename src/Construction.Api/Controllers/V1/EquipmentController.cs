using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Equipment;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Equipment.AssignEquipment;
using Construction.Application.Equipment.ChangeEquipmentStatus;
using Construction.Application.Equipment.CreateEquipment;
using Construction.Application.Equipment.GetEquipment;
using Construction.Application.Equipment.GetEquipmentList;
using Construction.Application.Equipment.Maintenance;
using Construction.Application.Equipment.ReturnEquipment;
using Construction.Application.Equipment.UpdateEquipment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/equipment")]
public sealed class EquipmentController(
    CreateEquipmentCommandHandler createHandler,
    GetEquipmentQueryHandler getHandler,
    GetEquipmentListQueryHandler listHandler,
    UpdateEquipmentCommandHandler updateHandler,
    AssignEquipmentCommandHandler assignHandler,
    ReturnEquipmentCommandHandler returnHandler,
    ScheduleEquipmentMaintenanceCommandHandler scheduleMaintenanceHandler,
    CompleteEquipmentMaintenanceCommandHandler completeMaintenanceHandler,
    ChangeEquipmentStatusCommandHandler statusHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Equipment.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateEquipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateEquipmentCommand(
                projectId,
                request.AssetCode,
                request.Name,
                request.Make,
                request.Model,
                request.SerialNumber),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId, equipmentId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{equipmentId:guid}")]
    [HasPermission(Permissions.Equipment.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid equipmentId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetEquipmentQuery(projectId, equipmentId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Equipment.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetEquipmentListQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{equipmentId:guid}")]
    [HasPermission(Permissions.Equipment.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid equipmentId,
        UpdateEquipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateEquipmentCommand(
                projectId,
                equipmentId,
                request.Name,
                request.Make,
                request.Model,
                request.SerialNumber),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{equipmentId:guid}/assign")]
    [HasPermission(Permissions.Equipment.Manage)]
    public async Task<IActionResult> AssignAsync(
        Guid projectId,
        Guid equipmentId,
        AssignEquipmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await assignHandler.HandleAsync(
            new AssignEquipmentCommand(
                projectId,
                equipmentId,
                request.UserId,
                request.LocationId,
                request.Notes),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }

    [HttpPost("{equipmentId:guid}/return")]
    [HasPermission(Permissions.Equipment.Manage)]
    public async Task<IActionResult> ReturnAsync(
        Guid projectId,
        Guid equipmentId,
        CancellationToken cancellationToken)
    {
        var result = await returnHandler.HandleAsync(
            new ReturnEquipmentCommand(projectId, equipmentId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{equipmentId:guid}/maintenance")]
    [HasPermission(Permissions.Equipment.Manage)]
    public async Task<IActionResult> ScheduleMaintenanceAsync(
        Guid projectId,
        Guid equipmentId,
        ScheduleEquipmentMaintenanceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await scheduleMaintenanceHandler.HandleAsync(
            new ScheduleEquipmentMaintenanceCommand(
                projectId,
                equipmentId,
                request.Description,
                request.ScheduledDate,
                request.ServiceProvider),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }

    [HttpPost("{equipmentId:guid}/maintenance/{maintenanceRecordId:guid}/complete")]
    [HasPermission(Permissions.Equipment.Manage)]
    public async Task<IActionResult> CompleteMaintenanceAsync(
        Guid projectId,
        Guid equipmentId,
        Guid maintenanceRecordId,
        CompleteEquipmentMaintenanceRequest request,
        CancellationToken cancellationToken)
    {
        var result = await completeMaintenanceHandler.HandleAsync(
            new CompleteEquipmentMaintenanceCommand(
                projectId,
                equipmentId,
                maintenanceRecordId,
                request.CompletedDate,
                request.Cost,
                request.CompletionNotes),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{equipmentId:guid}/status")]
    [HasPermission(Permissions.Equipment.Manage)]
    public async Task<IActionResult> ChangeStatusAsync(
        Guid projectId,
        Guid equipmentId,
        ChangeEquipmentStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await statusHandler.HandleAsync(
            new ChangeEquipmentStatusCommand(
                projectId,
                equipmentId,
                request.Status),
            cancellationToken);

        return result.ToActionResult();
    }
}
