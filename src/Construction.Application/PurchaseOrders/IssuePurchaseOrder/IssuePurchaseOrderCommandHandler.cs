using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.PurchaseOrders.IssuePurchaseOrder;

public sealed class IssuePurchaseOrderCommandHandler(
    IPurchaseOrderRepository orders,
    IPurchaseRequestRepository requests,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<IssuePurchaseOrderCommand, PurchaseOrderResponse>
{
    public async Task<Result<PurchaseOrderResponse>> HandleAsync(
        IssuePurchaseOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            command.ProjectId,
            Permissions.Procurement.IssueOrders,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PurchaseOrderResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.Unauthorized("Authentication.Required", "Authentication is required."));
        }

        var order = await orders.GetAsync(command.PurchaseOrderId, cancellationToken);

        if (order is null || order.ProjectId != command.ProjectId)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.NotFound("PurchaseOrders.NotFound", "The purchase order was not found."));
        }

        order.Issue(userId, timeProvider.GetUtcNow());

        if (order.PurchaseRequestId is Guid requestId)
        {
            var request = await requests.GetAsync(requestId, cancellationToken);

            if (request is null || request.ProjectId != command.ProjectId)
            {
                return Result.Failure<PurchaseOrderResponse>(
                    ApplicationError.Conflict(
                        "PurchaseOrders.LinkedRequestMissing",
                        "The linked purchase request is unavailable."));
            }

            request.MarkConverted();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseOrderResponse.FromDomain(order));
    }
}
