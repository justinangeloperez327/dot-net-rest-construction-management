using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Inspections;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Inspections.CancelInspection;
using Construction.Application.Inspections.CreateInspection;
using Construction.Application.Inspections.GetInspection;
using Construction.Application.Inspections.GetInspections;
using Construction.Application.Inspections.PerformInspection;
using Construction.Application.Inspections.RequestInspection;
using Construction.Application.Inspections.UpdateInspection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/inspections")]
public sealed class InspectionsController(
    CreateInspectionCommandHandler createHandler,
    GetInspectionQueryHandler getHandler,
    GetInspectionsQueryHandler listHandler,
    UpdateInspectionCommandHandler updateHandler,
    RequestInspectionCommandHandler requestHandler,
    StartInspectionCommandHandler startHandler,
    CompleteInspectionCommandHandler completeHandler,
    CancelInspectionCommandHandler cancelHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Inspections.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateInspectionCommand(
                projectId,
                request.Number,
                request.Title,
                request.Description,
                request.Type,
                request.LocationId,
                request.ActivityId,
                request.RequestedForDate,
                request.InspectorUserId),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId, inspectionId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{inspectionId:guid}")]
    [HasPermission(Permissions.Inspections.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid inspectionId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetInspectionQuery(projectId, inspectionId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Inspections.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetInspectionsQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{inspectionId:guid}")]
    [HasPermission(Permissions.Inspections.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid inspectionId,
        UpdateInspectionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateInspectionCommand(
                projectId,
                inspectionId,
                request.Title,
                request.Description,
                request.Type,
                request.LocationId,
                request.ActivityId,
                request.RequestedForDate,
                request.InspectorUserId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{inspectionId:guid}/request")]
    [HasPermission(Permissions.Inspections.Manage)]
    public async Task<IActionResult> RequestAsync(
        Guid projectId,
        Guid inspectionId,
        CancellationToken cancellationToken)
    {
        var result = await requestHandler.HandleAsync(
            new RequestInspectionCommand(projectId, inspectionId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{inspectionId:guid}/start")]
    [HasPermission(Permissions.Inspections.Perform)]
    public async Task<IActionResult> StartAsync(
        Guid projectId,
        Guid inspectionId,
        CancellationToken cancellationToken)
    {
        var result = await startHandler.HandleAsync(
            new StartInspectionCommand(projectId, inspectionId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{inspectionId:guid}/complete")]
    [HasPermission(Permissions.Inspections.Perform)]
    public async Task<IActionResult> CompleteAsync(
        Guid projectId,
        Guid inspectionId,
        CompleteInspectionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await completeHandler.HandleAsync(
            new CompleteInspectionCommand(
                projectId,
                inspectionId,
                request.Passed,
                request.ResultNotes),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{inspectionId:guid}/cancel")]
    [HasPermission(Permissions.Inspections.Manage)]
    public async Task<IActionResult> CancelAsync(
        Guid projectId,
        Guid inspectionId,
        CancelInspectionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await cancelHandler.HandleAsync(
            new CancelInspectionCommand(
                projectId,
                inspectionId,
                request.Reason),
            cancellationToken);

        return result.ToActionResult();
    }
}
