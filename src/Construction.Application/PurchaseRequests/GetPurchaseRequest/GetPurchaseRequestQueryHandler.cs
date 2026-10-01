using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.PurchaseRequests.GetPurchaseRequest;

public sealed class GetPurchaseRequestQueryHandler(
    IPurchaseRequestRepository requests,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetPurchaseRequestQuery, PurchaseRequestResponse>
{
    public async Task<Result<PurchaseRequestResponse>> HandleAsync(
        GetPurchaseRequestQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError = await ProjectAccessGuard.CheckAsync(
            currentUser,
            projectAccessService,
            query.ProjectId,
            Permissions.Procurement.View,
            cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<PurchaseRequestResponse>(accessError);
        }

        var request = await requests.GetAsync(query.PurchaseRequestId, cancellationToken);

        return request is null || request.ProjectId != query.ProjectId
            ? Result.Failure<PurchaseRequestResponse>(
                ApplicationError.NotFound("PurchaseRequests.NotFound", "The purchase request was not found."))
            : Result.Success(PurchaseRequestResponse.FromDomain(request));
    }
}
