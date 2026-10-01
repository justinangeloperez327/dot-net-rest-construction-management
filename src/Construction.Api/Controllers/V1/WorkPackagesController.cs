using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Activities;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.WorkPackages.ChangeWorkPackageStatus;
using Construction.Application.WorkPackages.CreateWorkPackage;
using Construction.Application.WorkPackages.GetWorkPackage;
using Construction.Application.WorkPackages.GetWorkPackages;
using Construction.Application.WorkPackages.UpdateWorkPackage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/work-packages")]
public sealed class WorkPackagesController(
    CreateWorkPackageCommandHandler createHandler,
    GetWorkPackageQueryHandler getHandler,
    GetWorkPackagesQueryHandler listHandler,
    UpdateWorkPackageCommandHandler updateHandler,
    ChangeWorkPackageStatusCommandHandler statusHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateWorkPackageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateWorkPackageCommand(
                projectId,
                request.Code,
                request.Name,
                request.Description,
                request.ParentWorkPackageId),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new
                {
                    projectId,
                    workPackageId = result.Value.Id
                },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{workPackageId:guid}")]
    [HasPermission(Permissions.Activities.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid workPackageId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetWorkPackageQuery(projectId, workPackageId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Activities.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await listHandler.HandleAsync(
            new GetWorkPackagesQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{workPackageId:guid}")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid workPackageId,
        UpdateWorkPackageRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateWorkPackageCommand(
                projectId,
                workPackageId,
                request.Name,
                request.Description,
                request.ParentWorkPackageId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{workPackageId:guid}/status")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> ChangeStatusAsync(
        Guid projectId,
        Guid workPackageId,
        ChangeWorkPackageStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await statusHandler.HandleAsync(
            new ChangeWorkPackageStatusCommand(
                projectId,
                workPackageId,
                request.Status),
            cancellationToken);

        return result.ToActionResult();
    }
}
