using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Locations;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Locations.CreateLocation;
using Construction.Application.Locations.GetLocations;
using Construction.Application.Locations.UpdateLocation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/locations")]
public sealed class ProjectLocationsController(
    CreateLocationCommandHandler createHandler,
    GetLocationsQueryHandler listHandler,
    UpdateLocationCommandHandler updateHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Projects.Update)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateProjectLocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateLocationCommand(
                projectId,
                request.Name,
                request.Type,
                request.ParentLocationId),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(
                StatusCodes.Status201Created,
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Projects.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await listHandler.HandleAsync(
            new GetLocationsQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{locationId:guid}")]
    [HasPermission(Permissions.Projects.Update)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid locationId,
        UpdateProjectLocationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateLocationCommand(
                projectId,
                locationId,
                request.Name,
                request.Type,
                request.ParentLocationId,
                request.IsActive),
            cancellationToken);

        return result.ToActionResult();
    }
}
