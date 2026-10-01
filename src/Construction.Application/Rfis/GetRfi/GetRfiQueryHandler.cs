using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Rfis.GetRfi;

public sealed class GetRfiQueryHandler(
    IRfiRepository rfis,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetRfiQuery, RfiResponse>
{
    public async Task<Result<RfiResponse>> HandleAsync(
        GetRfiQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Rfis.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<RfiResponse>(accessError);
        }

        var rfi = await rfis.GetAsync(query.RfiId, cancellationToken);

        return rfi is null || rfi.ProjectId != query.ProjectId
            ? Result.Failure<RfiResponse>(
                ApplicationError.NotFound(
                    "Rfis.NotFound",
                    "The RFI was not found."))
            : Result.Success(RfiResponse.FromDomain(rfi));
    }
}
