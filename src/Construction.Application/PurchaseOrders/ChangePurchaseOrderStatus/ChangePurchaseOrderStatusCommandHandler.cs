using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.PurchaseOrders;

namespace Construction.Application.PurchaseOrders.ChangePurchaseOrderStatus;

public sealed class ChangePurchaseOrderStatusCommandHandler(
    IPurchaseOrderRepository orders,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<ChangePurchaseOrderStatusCommand, PurchaseOrderResponse>
{
    public async Task<Result<PurchaseOrderResponse>> HandleAsync(
        ChangePurchaseOrderStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            command.ProjectId,
            Permissions.Procurement.ManageOrders,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PurchaseOrderResponse>(accessError);
        }

        var order = await orders.GetAsync(command.PurchaseOrderId, cancellationToken);

        if (order is null || order.ProjectId != command.ProjectId)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.NotFound("PurchaseOrders.NotFound", "The purchase order was not found."));
        }

        switch (command.Status)
        {
            case PurchaseOrderStatus.Closed:
                order.Close();
                break;
            case PurchaseOrderStatus.Cancelled:
                order.Cancel();
                break;
            default:
                return Result.Failure<PurchaseOrderResponse>(
                    ApplicationError.Validation(
                        "PurchaseOrders.InvalidStatusTransition",
                        "The requested purchase order status transition is not supported."));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseOrderResponse.FromDomain(order));
    }
}
