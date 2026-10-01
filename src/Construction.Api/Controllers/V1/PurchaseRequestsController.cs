using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.PurchaseRequests;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.PurchaseRequests.CancelPurchaseRequest;
using Construction.Application.PurchaseRequests.CreatePurchaseRequest;
using Construction.Application.PurchaseRequests.GetPurchaseRequest;
using Construction.Application.PurchaseRequests.GetPurchaseRequests;
using Construction.Application.PurchaseRequests.ReviewPurchaseRequest;
using Construction.Application.PurchaseRequests.SubmitPurchaseRequest;
using Construction.Application.PurchaseRequests.UpdatePurchaseRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/purchase-requests")]
public sealed class PurchaseRequestsController(
    CreatePurchaseRequestCommandHandler createHandler,
    GetPurchaseRequestQueryHandler getHandler,
    GetPurchaseRequestsQueryHandler listHandler,
    UpdatePurchaseRequestCommandHandler updateHandler,
    SubmitPurchaseRequestCommandHandler submitHandler,
    ReviewPurchaseRequestCommandHandler reviewHandler,
    CancelPurchaseRequestCommandHandler cancelHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Procurement.ManageRequests)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreatePurchaseRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreatePurchaseRequestCommand(
                projectId,
                request.Number,
                request.Title,
                request.CurrencyCode,
                request.Items.Select(item => new PurchaseRequestItemInputModel(
                    item.Description,
                    item.Quantity,
                    item.Unit,
                    item.EstimatedUnitCost)).ToArray()),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId, purchaseRequestId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{purchaseRequestId:guid}")]
    [HasPermission(Permissions.Procurement.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid purchaseRequestId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetPurchaseRequestQuery(projectId, purchaseRequestId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpGet]
    [HasPermission(Permissions.Procurement.View)]
    public async Task<IActionResult> ListAsync(
        Guid projectId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 25,
        CancellationToken cancellationToken = default)
    {
        var result = await listHandler.HandleAsync(
            new GetPurchaseRequestsQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{purchaseRequestId:guid}")]
    [HasPermission(Permissions.Procurement.ManageRequests)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid purchaseRequestId,
        UpdatePurchaseRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdatePurchaseRequestCommand(
                projectId,
                purchaseRequestId,
                request.Title,
                request.CurrencyCode,
                request.Items.Select(item => new PurchaseRequestItemInputModel(
                    item.Description,
                    item.Quantity,
                    item.Unit,
                    item.EstimatedUnitCost)).ToArray()),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{purchaseRequestId:guid}/submit")]
    [HasPermission(Permissions.Procurement.ManageRequests)]
    public async Task<IActionResult> SubmitAsync(
        Guid projectId,
        Guid purchaseRequestId,
        CancellationToken cancellationToken)
    {
        var result = await submitHandler.HandleAsync(
            new SubmitPurchaseRequestCommand(projectId, purchaseRequestId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{purchaseRequestId:guid}/review")]
    [HasPermission(Permissions.Procurement.ApproveRequests)]
    public async Task<IActionResult> ReviewAsync(
        Guid projectId,
        Guid purchaseRequestId,
        ReviewPurchaseRequestRequest request,
        CancellationToken cancellationToken)
    {
        var result = await reviewHandler.HandleAsync(
            new ReviewPurchaseRequestCommand(
                projectId,
                purchaseRequestId,
                request.Approve,
                request.Remarks),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{purchaseRequestId:guid}/cancel")]
    [HasPermission(Permissions.Procurement.ManageRequests)]
    public async Task<IActionResult> CancelAsync(
        Guid projectId,
        Guid purchaseRequestId,
        CancellationToken cancellationToken)
    {
        var result = await cancelHandler.HandleAsync(
            new CancelPurchaseRequestCommand(projectId, purchaseRequestId),
            cancellationToken);

        return result.ToActionResult();
    }
}
