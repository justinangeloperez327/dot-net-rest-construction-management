using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.PurchaseRequests.ReviewPurchaseRequest;

public sealed class ReviewPurchaseRequestCommandHandler(
    IPurchaseRequestRepository requests,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ReviewPurchaseRequestCommand, PurchaseRequestResponse>
{
    public async Task<Result<PurchaseRequestResponse>> HandleAsync(
        ReviewPurchaseRequestCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            command.ProjectId,
            Permissions.Procurement.ApproveRequests,
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

        var request = await requests.GetAsync(command.PurchaseRequestId, cancellationToken);

        if (request is null || request.ProjectId != command.ProjectId)
        {
            return Result.Failure<PurchaseRequestResponse>(
                ApplicationError.NotFound("PurchaseRequests.NotFound", "The purchase request was not found."));
        }

        if (command.Approve)
        {
            request.Approve(userId, timeProvider.GetUtcNow(), command.Remarks);
        }
        else
        {
            request.Reject(
                userId,
                timeProvider.GetUtcNow(),
                command.Remarks ?? string.Empty);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(PurchaseRequestResponse.FromDomain(request));
    }
}
