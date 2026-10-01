using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Activities;
using Construction.Api.Extensions;
using Construction.Application.Activities.Assignments;
using Construction.Application.Activities.ChangeActivityStatus;
using Construction.Application.Activities.CreateActivity;
using Construction.Application.Activities.Dependencies;
using Construction.Application.Activities.GetActivities;
using Construction.Application.Activities.GetActivity;
using Construction.Application.Activities.UpdateActivity;
using Construction.Application.Activities.UpdateActivityProgress;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/activities")]
public sealed class ActivitiesController(
    CreateActivityCommandHandler createHandler,
    GetActivityQueryHandler getHandler,
    GetActivitiesQueryHandler listHandler,
    UpdateActivityCommandHandler updateHandler,
    UpdateActivityProgressCommandHandler progressHandler,
    ChangeActivityStatusCommandHandler statusHandler,
    AddActivityAssignmentCommandHandler addAssignmentHandler,
    GetActivityAssignmentsQueryHandler assignmentsHandler,
    RemoveActivityAssignmentCommandHandler removeAssignmentHandler,
    AddActivityDependencyCommandHandler addDependencyHandler,
    GetActivityDependenciesQueryHandler dependenciesHandler,
    RemoveActivityDependencyCommandHandler removeDependencyHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateActivityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateActivityCommand(
                projectId,
                request.Code,
                request.Name,
                request.Description,
                request.WorkPackageId,
                request.LocationId,
                request.Priority,
                request.PlannedStartDate,
                request.PlannedEndDate),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new
                {
                    projectId,
                    activityId = result.Value.Id
                },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{activityId:guid}")]
    [HasPermission(Permissions.Activities.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid activityId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetActivityQuery(projectId, activityId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Activities.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetActivitiesQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{activityId:guid}")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid activityId,
        UpdateActivityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateActivityCommand(
                projectId,
                activityId,
                request.Name,
                request.Description,
                request.WorkPackageId,
                request.LocationId,
                request.Priority,
                request.PlannedStartDate,
                request.PlannedEndDate),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{activityId:guid}/progress")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> UpdateProgressAsync(
        Guid projectId,
        Guid activityId,
        UpdateActivityProgressRequest request,
        CancellationToken cancellationToken)
    {
        var result = await progressHandler.HandleAsync(
            new UpdateActivityProgressCommand(
                projectId,
                activityId,
                request.ProgressPercentage),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{activityId:guid}/status")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> ChangeStatusAsync(
        Guid projectId,
        Guid activityId,
        ChangeActivityStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await statusHandler.HandleAsync(
            new ChangeActivityStatusCommand(
                projectId,
                activityId,
                request.Status,
                request.EffectiveDate),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{activityId:guid}/assignments")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> AddAssignmentAsync(
        Guid projectId,
        Guid activityId,
        AddActivityAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addAssignmentHandler.HandleAsync(
            new AddActivityAssignmentCommand(
                projectId,
                activityId,
                request.UserId,
                request.Role),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{activityId:guid}/assignments")]
    [HasPermission(Permissions.Activities.View)]
    public async Task<IActionResult> GetAssignmentsAsync(
        Guid projectId,
        Guid activityId,
        CancellationToken cancellationToken)
    {
        var result = await assignmentsHandler.HandleAsync(
            new GetActivityAssignmentsQuery(projectId, activityId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{activityId:guid}/assignments/{assignmentId:guid}")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> RemoveAssignmentAsync(
        Guid projectId,
        Guid activityId,
        Guid assignmentId,
        CancellationToken cancellationToken)
    {
        var result = await removeAssignmentHandler.HandleAsync(
            new RemoveActivityAssignmentCommand(
                projectId,
                activityId,
                assignmentId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{activityId:guid}/dependencies")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> AddDependencyAsync(
        Guid projectId,
        Guid activityId,
        AddActivityDependencyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addDependencyHandler.HandleAsync(
            new AddActivityDependencyCommand(
                projectId,
                activityId,
                request.PredecessorActivityId,
                request.Type,
                request.LagDays),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{activityId:guid}/dependencies")]
    [HasPermission(Permissions.Activities.View)]
    public async Task<IActionResult> GetDependenciesAsync(
        Guid projectId,
        Guid activityId,
        CancellationToken cancellationToken)
    {
        var result = await dependenciesHandler.HandleAsync(
            new GetActivityDependenciesQuery(projectId, activityId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpDelete("{activityId:guid}/dependencies/{dependencyId:guid}")]
    [HasPermission(Permissions.Activities.Manage)]
    public async Task<IActionResult> RemoveDependencyAsync(
        Guid projectId,
        Guid activityId,
        Guid dependencyId,
        CancellationToken cancellationToken)
    {
        var result = await removeDependencyHandler.HandleAsync(
            new RemoveActivityDependencyCommand(
                projectId,
                activityId,
                dependencyId),
            cancellationToken);

        return result.ToActionResult();
    }
}
