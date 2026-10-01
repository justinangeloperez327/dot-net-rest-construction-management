using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.Issues;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.Issues.CancelIssue;
using Construction.Application.Issues.CorrectiveActions;
using Construction.Application.Issues.CreateIssue;
using Construction.Application.Issues.GetIssue;
using Construction.Application.Issues.GetIssues;
using Construction.Application.Issues.StartIssue;
using Construction.Application.Issues.SubmitIssueForVerification;
using Construction.Application.Issues.UpdateIssue;
using Construction.Application.Issues.VerifyIssue;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/issues")]
public sealed class IssuesController(
    CreateIssueCommandHandler createHandler,
    GetIssueQueryHandler getHandler,
    GetIssuesQueryHandler listHandler,
    UpdateIssueCommandHandler updateHandler,
    StartIssueCommandHandler startHandler,
    AddCorrectiveActionCommandHandler addCorrectiveActionHandler,
    UpdateCorrectiveActionCommandHandler updateCorrectiveActionHandler,
    SubmitIssueForVerificationCommandHandler submitHandler,
    VerifyIssueCommandHandler verifyHandler,
    CancelIssueCommandHandler cancelHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Issues.Manage)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreateIssueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreateIssueCommand(
                projectId,
                request.Number,
                request.Title,
                request.Description,
                request.Type,
                request.Severity,
                request.LocationId,
                request.ActivityId,
                request.ResponsibleUserId,
                request.DueDate),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId, issueId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{issueId:guid}")]
    [HasPermission(Permissions.Issues.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid issueId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetIssueQuery(projectId, issueId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Issues.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetIssuesQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{issueId:guid}")]
    [HasPermission(Permissions.Issues.Manage)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid issueId,
        UpdateIssueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdateIssueCommand(
                projectId,
                issueId,
                request.Title,
                request.Description,
                request.Type,
                request.Severity,
                request.LocationId,
                request.ActivityId,
                request.ResponsibleUserId,
                request.DueDate),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{issueId:guid}/start")]
    [HasPermission(Permissions.Issues.Manage)]
    public async Task<IActionResult> StartAsync(
        Guid projectId,
        Guid issueId,
        CancellationToken cancellationToken)
    {
        var result = await startHandler.HandleAsync(
            new StartIssueCommand(projectId, issueId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{issueId:guid}/corrective-actions")]
    [HasPermission(Permissions.Issues.Manage)]
    public async Task<IActionResult> AddCorrectiveActionAsync(
        Guid projectId,
        Guid issueId,
        AddCorrectiveActionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await addCorrectiveActionHandler.HandleAsync(
            new AddCorrectiveActionCommand(
                projectId,
                issueId,
                request.Description,
                request.ResponsibleUserId,
                request.DueDate),
            cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : result.ToActionResult();
    }

    [HttpPut("{issueId:guid}/corrective-actions/{correctiveActionId:guid}")]
    [HasPermission(Permissions.Issues.Manage)]
    public async Task<IActionResult> UpdateCorrectiveActionAsync(
        Guid projectId,
        Guid issueId,
        Guid correctiveActionId,
        UpdateCorrectiveActionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateCorrectiveActionHandler.HandleAsync(
            new UpdateCorrectiveActionCommand(
                projectId,
                issueId,
                correctiveActionId,
                request.Description,
                request.ResponsibleUserId,
                request.DueDate,
                request.Status,
                request.CompletionNotes),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{issueId:guid}/submit-for-verification")]
    [HasPermission(Permissions.Issues.Manage)]
    public async Task<IActionResult> SubmitForVerificationAsync(
        Guid projectId,
        Guid issueId,
        SubmitIssueForVerificationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await submitHandler.HandleAsync(
            new SubmitIssueForVerificationCommand(
                projectId,
                issueId,
                request.ResolutionSummary),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{issueId:guid}/verify")]
    [HasPermission(Permissions.Issues.Verify)]
    public async Task<IActionResult> VerifyAsync(
        Guid projectId,
        Guid issueId,
        VerifyIssueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await verifyHandler.HandleAsync(
            new VerifyIssueCommand(
                projectId,
                issueId,
                request.Close,
                request.ReopenReason),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{issueId:guid}/cancel")]
    [HasPermission(Permissions.Issues.Manage)]
    public async Task<IActionResult> CancelAsync(
        Guid projectId,
        Guid issueId,
        CancelIssueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await cancelHandler.HandleAsync(
            new CancelIssueCommand(
                projectId,
                issueId,
                request.Reason),
            cancellationToken);

        return result.ToActionResult();
    }
}
