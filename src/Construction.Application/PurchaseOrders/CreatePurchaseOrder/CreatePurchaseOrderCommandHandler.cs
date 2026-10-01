using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.PurchaseOrders;
using Construction.Domain.PurchaseRequests;
using Construction.Domain.Suppliers;

namespace Construction.Application.PurchaseOrders.CreatePurchaseOrder;

public sealed class CreatePurchaseOrderCommandHandler(
    IProjectRepository projects,
    ISupplierRepository suppliers,
    IPurchaseRequestRepository requests,
    IPurchaseOrderRepository orders,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CreatePurchaseOrderCommand, PurchaseOrderResponse>
{
    public async Task<Result<PurchaseOrderResponse>> HandleAsync(
        CreatePurchaseOrderCommand command,
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

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.Unauthorized("Authentication.Required", "Authentication is required."));
        }

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.NotFound("Projects.NotFound", "The project was not found."));
        }

        Supplier? supplier = await suppliers.GetAsync(command.SupplierId, cancellationToken);

        if (supplier is null || supplier.Status != SupplierStatus.Active)
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.Validation(
                    "PurchaseOrders.InvalidSupplier",
                    "Purchase order supplier must be active."));
        }

        if (command.PurchaseRequestId is Guid requestId)
        {
            PurchaseRequest? request = await requests.GetAsync(requestId, cancellationToken);

            if (request is null
                || request.ProjectId != command.ProjectId
                || request.Status != PurchaseRequestStatus.Approved)
            {
                return Result.Failure<PurchaseOrderResponse>(
                    ApplicationError.Validation(
                        "PurchaseOrders.InvalidPurchaseRequest",
                        "Linked purchase request must be approved and belong to the same project."));
            }

            if (await orders.ExistsForPurchaseRequestAsync(requestId, cancellationToken))
            {
                return Result.Failure<PurchaseOrderResponse>(
                    ApplicationError.Conflict(
                        "PurchaseOrders.RequestAlreadyLinked",
                        "The purchase request is already linked to a purchase order."));
            }
        }

        if (await orders.ExistsByNumberAsync(
            command.ProjectId,
            command.Number.Trim().ToUpperInvariant(),
            cancellationToken))
        {
            return Result.Failure<PurchaseOrderResponse>(
                ApplicationError.Conflict(
                    "PurchaseOrders.NumberAlreadyExists",
                    "A purchase order with the same number already exists in this project."));
        }

        PurchaseOrder order = PurchaseOrder.Create(
            command.ProjectId,
            command.SupplierId,
            command.PurchaseRequestId,
            command.Number,
            command.CurrencyCode,
            command.ExpectedDeliveryDate,
            userId,
            command.Items.Select(item => new PurchaseOrderItemInput(
                item.Description,
                item.OrderedQuantity,
                item.Unit,
                item.UnitPrice)));

        orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseOrderResponse.FromDomain(order));
    }
}
