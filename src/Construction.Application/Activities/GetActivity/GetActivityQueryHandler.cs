using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.GetActivity;

public sealed class GetActivityQueryHandler(
    IActivityRepository activities,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetActivityQuery, ActivityResponse>
{
    public async Task<Result<ActivityResponse>> HandleAsync(
        GetActivityQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Activities.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ActivityResponse>(accessError);
        }

        var activity = await activities.GetAsync(
            query.ActivityId,
            cancellationToken);

        return activity is null || activity.ProjectId != query.ProjectId
            ? Result.Failure<ActivityResponse>(
                ApplicationError.NotFound(
                    "Activities.NotFound",
                    "The activity was not found."))
            : Result.Success(ActivityResponse.FromDomain(activity));
    }
}
