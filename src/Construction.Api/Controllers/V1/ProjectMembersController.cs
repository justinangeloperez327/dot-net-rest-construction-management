using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.ProjectMembers;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.ProjectMembers.AddProjectMember;
using Construction.Application.ProjectMembers.GetProjectMembers;
using Construction.Application.ProjectMembers.UpdateProjectMember;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/members")]
public sealed class ProjectMembersController(
    AddProjectMemberCommandHandler addHandler,
    GetProjectMembersQueryHandler listHandler,
    UpdateProjectMemberCommandHandler updateHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Projects.ManageMembers)]
    public async Task<IActionResult> AddAsync(
        Guid projectId,
        AddProjectMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addHandler.HandleAsync(
            new AddProjectMemberCommand(
                projectId,
                request.UserId,
                request.Role),
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
            new GetProjectMembersQuery(projectId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{memberId:guid}")]
    [HasPermission(Permissions.Projects.ManageMembers)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid memberId,
        UpdateProjectMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateProjectMemberCommand(
                projectId,
                memberId,
                request.Role,
                request.IsActive),
            cancellationToken);

        return result.ToActionResult();
    }
}
