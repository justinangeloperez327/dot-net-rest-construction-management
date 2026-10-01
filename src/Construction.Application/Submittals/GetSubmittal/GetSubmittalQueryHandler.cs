using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Submittals.GetSubmittal;

public sealed class GetSubmittalQueryHandler(
    ISubmittalRepository submittals,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetSubmittalQuery, SubmittalResponse>
{
    public async Task<Result<SubmittalResponse>> HandleAsync(
        GetSubmittalQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Submittals.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<SubmittalResponse>(accessError);
        }

        var submittal = await submittals.GetAsync(
            query.SubmittalId,
            cancellationToken);

        return submittal is null || submittal.ProjectId != query.ProjectId
            ? Result.Failure<SubmittalResponse>(
                ApplicationError.NotFound(
                    "Submittals.NotFound",
                    "The submittal was not found."))
            : Result.Success(SubmittalResponse.FromDomain(submittal));
    }
}
