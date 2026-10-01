using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Rfis;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Rfis.AddRfiComment;
using Construction.Application.Rfis.ChangeRfiStatus;
using Construction.Application.Rfis.CreateRfi;
using Construction.Application.Rfis.GetRfi;
using Construction.Application.Rfis.GetRfis;
using Construction.Application.Rfis.OpenRfi;
using Construction.Application.Rfis.RespondRfi;
using Construction.Application.Rfis.UpdateRfi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/rfis")]
public sealed class RfisController(
    CreateRfiCommandHandler createHandler,
    GetRfiQueryHandler getHandler,
    GetRfisQueryHandler listHandler,
    UpdateRfiCommandHandler updateHandler,
    OpenRfiCommandHandler openHandler,
    RespondRfiCommandHandler respondHandler,
    ChangeRfiStatusCommandHandler statusHandler,
    AddRfiCommentCommandHandler commentHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Rfis.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateRfiRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateRfiCommand(
                projectId,
                request.Number,
                request.Subject,
                request.Question,
                request.DueDate,
                request.ResponsibleUserId),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId, rfiId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{rfiId:guid}")]
    [HasPermission(Permissions.Rfis.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid rfiId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetRfiQuery(projectId, rfiId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Rfis.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetRfisQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{rfiId:guid}")]
    [HasPermission(Permissions.Rfis.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid rfiId,
        UpdateRfiRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateRfiCommand(
                projectId,
                rfiId,
                request.Subject,
                request.Question,
                request.DueDate,
                request.ResponsibleUserId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{rfiId:guid}/open")]
    [HasPermission(Permissions.Rfis.Manage)]
    public async Task<IActionResult> OpenAsync(
        Guid projectId,
        Guid rfiId,
        CancellationToken cancellationToken)
    {
        var result = await openHandler.HandleAsync(
            new OpenRfiCommand(projectId, rfiId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{rfiId:guid}/response")]
    [HasPermission(Permissions.Rfis.Respond)]
    public async Task<IActionResult> RespondAsync(
        Guid projectId,
        Guid rfiId,
        RespondRfiRequest request,
        CancellationToken cancellationToken)
    {
        var result = await respondHandler.HandleAsync(
            new RespondRfiCommand(
                projectId,
                rfiId,
                request.Response),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{rfiId:guid}/status")]
    [HasPermission(Permissions.Rfis.Manage)]
    public async Task<IActionResult> ChangeStatusAsync(
        Guid projectId,
        Guid rfiId,
        ChangeRfiStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await statusHandler.HandleAsync(
            new ChangeRfiStatusCommand(
                projectId,
                rfiId,
                request.Status,
                request.Reason),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{rfiId:guid}/comments")]
    [HasPermission(Permissions.Rfis.Manage)]
    public async Task<IActionResult> AddCommentAsync(
        Guid projectId,
        Guid rfiId,
        AddRfiCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await commentHandler.HandleAsync(
            new AddRfiCommentCommand(
                projectId,
                rfiId,
                request.Body),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }
}
