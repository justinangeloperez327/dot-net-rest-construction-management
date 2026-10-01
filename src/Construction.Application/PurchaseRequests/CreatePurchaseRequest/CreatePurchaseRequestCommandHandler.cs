using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.PurchaseRequests;

namespace Construction.Application.PurchaseRequests.CreatePurchaseRequest;

public sealed class CreatePurchaseRequestCommandHandler(
    IProjectRepository projects,
    IPurchaseRequestRepository requests,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CreatePurchaseRequestCommand, PurchaseRequestResponse>
{
    public async Task<Result<PurchaseRequestResponse>> HandleAsync(
        CreatePurchaseRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            command.ProjectId,
            Permissions.Procurement.ManageRequests,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PurchaseRequestResponse>(accessError);
        }

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PurchaseRequestResponse>(
                ApplicationError.Unauthorized("Authentication.Required", "Authentication is required."));
        }

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<PurchaseRequestResponse>(
                ApplicationError.NotFound("Projects.NotFound", "The project was not found."));
        }

        if (await requests.ExistsByNumberAsync(
            command.ProjectId,
            command.Number.Trim().ToUpperInvariant(),
            cancellationToken))
        {
            return Result.Failure<PurchaseRequestResponse>(
                ApplicationError.Conflict(
                    "PurchaseRequests.NumberAlreadyExists",
                    "A purchase request with the same number already exists in this project."));
        }

        PurchaseRequest request = PurchaseRequest.Create(
            command.ProjectId,
            command.Number,
            command.Title,
            command.CurrencyCode,
            userId,
            command.Items.Select(item => new PurchaseRequestItemInput(
                item.Description,
                item.Quantity,
                item.Unit,
                item.EstimatedUnitCost)));

        requests.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseRequestResponse.FromDomain(request));
    }
}
