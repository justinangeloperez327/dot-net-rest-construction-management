using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Submittals;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Submittals.AddSubmittalComment;
using Construction.Application.Submittals.AddSubmittalRevision;
using Construction.Application.Submittals.ChangeSubmittalStatus;
using Construction.Application.Submittals.CreateSubmittal;
using Construction.Application.Submittals.GetSubmittal;
using Construction.Application.Submittals.GetSubmittals;
using Construction.Application.Submittals.ReviewSubmittal;
using Construction.Application.Submittals.StartSubmittalReview;
using Construction.Application.Submittals.SubmitSubmittal;
using Construction.Application.Submittals.UpdateSubmittal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/submittals")]
public sealed class SubmittalsController(
    CreateSubmittalCommandHandler createHandler,
    GetSubmittalQueryHandler getHandler,
    GetSubmittalsQueryHandler listHandler,
    UpdateSubmittalCommandHandler updateHandler,
    AddSubmittalRevisionCommandHandler revisionHandler,
    SubmitSubmittalCommandHandler submitHandler,
    StartSubmittalReviewCommandHandler startReviewHandler,
    ReviewSubmittalCommandHandler reviewHandler,
    ChangeSubmittalStatusCommandHandler statusHandler,
    AddSubmittalCommentCommandHandler commentHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Submittals.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateSubmittalRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateSubmittalCommand(
                projectId,
                request.Number,
                request.Title,
                request.Type,
                request.ResponsibleUserId),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId, submittalId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{submittalId:guid}")]
    [HasPermission(Permissions.Submittals.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid submittalId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetSubmittalQuery(projectId, submittalId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Submittals.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetSubmittalsQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{submittalId:guid}")]
    [HasPermission(Permissions.Submittals.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid submittalId,
        UpdateSubmittalRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateSubmittalCommand(
                projectId,
                submittalId,
                request.Title,
                request.Type,
                request.ResponsibleUserId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{submittalId:guid}/revisions")]
    [HasPermission(Permissions.Submittals.Manage)]
    public async Task<IActionResult> AddRevisionAsync(
        Guid projectId,
        Guid submittalId,
        AddSubmittalRevisionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await revisionHandler.HandleAsync(
            new AddSubmittalRevisionCommand(
                projectId,
                submittalId,
                request.RevisionCode,
                request.Description),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }

    [HttpPost("{submittalId:guid}/submit")]
    [HasPermission(Permissions.Submittals.Manage)]
    public async Task<IActionResult> SubmitAsync(
        Guid projectId,
        Guid submittalId,
        SubmitSubmittalRequest request,
        CancellationToken cancellationToken)
    {
        var result = await submitHandler.HandleAsync(
            new SubmitSubmittalCommand(
                projectId,
                submittalId,
                request.ReviewDueDate),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{submittalId:guid}/review/start")]
    [HasPermission(Permissions.Submittals.Review)]
    public async Task<IActionResult> StartReviewAsync(
        Guid projectId,
        Guid submittalId,
        CancellationToken cancellationToken)
    {
        var result = await startReviewHandler.HandleAsync(
            new StartSubmittalReviewCommand(
                projectId,
                submittalId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{submittalId:guid}/review")]
    [HasPermission(Permissions.Submittals.Review)]
    public async Task<IActionResult> ReviewAsync(
        Guid projectId,
        Guid submittalId,
        ReviewSubmittalRequest request,
        CancellationToken cancellationToken)
    {
        var result = await reviewHandler.HandleAsync(
            new ReviewSubmittalCommand(
                projectId,
                submittalId,
                request.Outcome,
                request.Remarks),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{submittalId:guid}/status")]
    [HasPermission(Permissions.Submittals.Manage)]
    public async Task<IActionResult> ChangeStatusAsync(
        Guid projectId,
        Guid submittalId,
        ChangeSubmittalStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await statusHandler.HandleAsync(
            new ChangeSubmittalStatusCommand(
                projectId,
                submittalId,
                request.Status,
                request.Reason),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{submittalId:guid}/comments")]
    [HasPermission(Permissions.Submittals.Manage)]
    public async Task<IActionResult> AddCommentAsync(
        Guid projectId,
        Guid submittalId,
        AddSubmittalCommentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await commentHandler.HandleAsync(
            new AddSubmittalCommentCommand(
                projectId,
                submittalId,
                request.Body),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }
}
