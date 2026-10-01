using Construction.Api.Authorization;
using Construction.Api.Configuration;
using Construction.Api.Contracts.PurchaseOrders;
using Construction.Api.Extensions;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Pagination;
using Construction.Application.PurchaseOrders.ChangePurchaseOrderStatus;
using Construction.Application.PurchaseOrders.CreatePurchaseOrder;
using Construction.Application.PurchaseOrders.GetPurchaseOrder;
using Construction.Application.PurchaseOrders.GetPurchaseOrders;
using Construction.Application.PurchaseOrders.IssuePurchaseOrder;
using Construction.Application.PurchaseOrders.ReceivePurchaseOrder;
using Construction.Application.PurchaseOrders.UpdatePurchaseOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Construction.Api.Controllers.V1;

[ApiController]
[Authorize]
[Route(ApiRoutes.Version1 + "/projects/{projectId:guid}/purchase-orders")]
public sealed class PurchaseOrdersController(
    CreatePurchaseOrderCommandHandler createHandler,
    GetPurchaseOrderQueryHandler getHandler,
    GetPurchaseOrdersQueryHandler listHandler,
    UpdatePurchaseOrderCommandHandler updateHandler,
    IssuePurchaseOrderCommandHandler issueHandler,
    ReceivePurchaseOrderCommandHandler receiveHandler,
    ChangePurchaseOrderStatusCommandHandler statusHandler)
    : ControllerBase
{
    [HttpPost]
    [HasPermission(Permissions.Procurement.ManageOrders)]
    public async Task<IActionResult> CreateAsync(
        Guid projectId,
        CreatePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await createHandler.HandleAsync(
            new CreatePurchaseOrderCommand(
                projectId,
                request.SupplierId,
                request.PurchaseRequestId,
                request.Number,
                request.CurrencyCode,
                request.ExpectedDeliveryDate,
                request.Items.Select(item => new PurchaseOrderItemInputModel(
                    item.Description,
                    item.OrderedQuantity,
                    item.Unit,
                    item.UnitPrice)).ToArray()),
            cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(
                nameof(GetAsync),
                new { projectId, purchaseOrderId = result.Value.Id },
                result.Value)
            : result.ToActionResult();
    }

    [HttpGet("{purchaseOrderId:guid}")]
    [HasPermission(Permissions.Procurement.View)]
    public async Task<IActionResult> GetAsync(
        Guid projectId,
        Guid purchaseOrderId,
        CancellationToken cancellationToken)
    {
        var result = await getHandler.HandleAsync(
            new GetPurchaseOrderQuery(projectId, purchaseOrderId),
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
            new GetPurchaseOrdersQuery(
                projectId,
                new PageRequest(pageNumber, pageSize)),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{purchaseOrderId:guid}")]
    [HasPermission(Permissions.Procurement.ManageOrders)]
    public async Task<IActionResult> UpdateAsync(
        Guid projectId,
        Guid purchaseOrderId,
        UpdatePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await updateHandler.HandleAsync(
            new UpdatePurchaseOrderCommand(
                projectId,
                purchaseOrderId,
                request.SupplierId,
                request.CurrencyCode,
                request.ExpectedDeliveryDate,
                request.Items.Select(item => new PurchaseOrderItemInputModel(
                    item.Description,
                    item.OrderedQuantity,
                    item.Unit,
                    item.UnitPrice)).ToArray()),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{purchaseOrderId:guid}/issue")]
    [HasPermission(Permissions.Procurement.IssueOrders)]
    public async Task<IActionResult> IssueAsync(
        Guid projectId,
        Guid purchaseOrderId,
        CancellationToken cancellationToken)
    {
        var result = await issueHandler.HandleAsync(
            new IssuePurchaseOrderCommand(projectId, purchaseOrderId),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost("{purchaseOrderId:guid}/receipts")]
    [HasPermission(Permissions.Procurement.ManageOrders)]
    public async Task<IActionResult> ReceiveAsync(
        Guid projectId,
        Guid purchaseOrderId,
        ReceivePurchaseOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await receiveHandler.HandleAsync(
            new ReceivePurchaseOrderCommand(
                projectId,
                purchaseOrderId,
                request.Receipts.Select(receipt => new PurchaseOrderReceiptInputModel(
                    receipt.ItemId,
                    receipt.Quantity)).ToArray()),
            cancellationToken);

        return result.ToActionResult();
    }

    [HttpPut("{purchaseOrderId:guid}/status")]
    [HasPermission(Permissions.Procurement.ManageOrders)]
    public async Task<IActionResult> ChangeStatusAsync(
        Guid projectId,
        Guid purchaseOrderId,
        ChangePurchaseOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await statusHandler.HandleAsync(
            new ChangePurchaseOrderStatusCommand(
                projectId,
                purchaseOrderId,
                request.Status),
            cancellationToken);

        return result.ToActionResult();
    }
}
