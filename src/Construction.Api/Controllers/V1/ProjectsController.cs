using Construction.Api.Authorization;
using Construction.Api.Contracts.Projects;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Projects.ChangeProjectStatus;
using Construction.Application.Projects.CreateProject;
using Construction.Application.Projects.GetProject;
using Construction.Application.Projects.GetProjects;
using Construction.Application.Projects.UpdateProject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[Authorize]
public sealed class ProjectsController(
    CreateProjectCommandHandler createHandler,
    GetProjectQueryHandler getHandler,
    GetProjectsQueryHandler listHandler,
    UpdateProjectCommandHandler updateHandler,
    ChangeProjectStatusCommandHandler statusHandler)
    : ApiControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Projects.Create)]
    public async Task<IActionResult> CreateAsync(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateProjectCommand(
                request.Number,
                request.Name,
                request.Description,
                request.StartDate,
                request.PlannedEndDate,
                request.ClientCompanyId,
                request.MainContractorCompanyId,
                request.ConsultantCompanyId),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{projectId:guid}")]
    [HasPermission(Permissions.Projects.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetProjectQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Projects.View)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetProjectsQuery(
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{projectId:guid}")]
    [HasPermission(Permissions.Projects.Update)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateProjectCommand(
                projectId,
                request.Name,
                request.Description,
                request.StartDate,
                request.PlannedEndDate,
                request.ClientCompanyId,
                request.MainContractorCompanyId,
                request.ConsultantCompanyId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{projectId:guid}/status")]
    public async Task<IActionResult> ChangeStatusAsync(
        Guid projectId,
        ChangeProjectStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await statusHandler.HandleAsync(
            new ChangeProjectStatusCommand(
                projectId,
                request.Status,
                request.CompletionDate),
            cancellationToken);

        return result.ToActionResult();
    }
}
