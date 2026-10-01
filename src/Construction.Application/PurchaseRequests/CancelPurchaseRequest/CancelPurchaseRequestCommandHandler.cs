using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.PurchaseRequests.CancelPurchaseRequest;

public sealed class CancelPurchaseRequestCommandHandler(
    IPurchaseRequestRepository requests,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CancelPurchaseRequestCommand, PurchaseRequestResponse>
{
    public async Task<Result<PurchaseRequestResponse>> HandleAsync(
        CancelPurchaseRequestCommand command,
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

        var request = await requests.GetAsync(command.PurchaseRequestId, cancellationToken);

        if (request is null || request.ProjectId != command.ProjectId)
        {
            return Result.Failure<PurchaseRequestResponse>(
                ApplicationError.NotFound("PurchaseRequests.NotFound", "The purchase request was not found."));
        }

        request.Cancel();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseRequestResponse.FromDomain(request));
    }
}
