using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.Dependencies;

public sealed class GetActivityDependenciesQueryHandler(
    IActivityRepository activities,
    IActivityDependencyRepository dependencies,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetActivityDependenciesQuery, IReadOnlyCollection<ActivityDependencyResponse>>
{
    public async Task<Result<IReadOnlyCollection<ActivityDependencyResponse>>> HandleAsync(
        GetActivityDependenciesQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Activities.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<IReadOnlyCollection<ActivityDependencyResponse>>(
                accessError);
        }

        var activity = await activities.GetAsync(
            query.ActivityId,
            cancellationToken);

        if (activity is null || activity.ProjectId != query.ProjectId)
        {
            return Result.Failure<IReadOnlyCollection<ActivityDependencyResponse>>(
                ApplicationError.NotFound(
                    "Activities.NotFound",
                    "The activity was not found."));
        }

        var items = await dependencies.GetByActivityAsync(
            query.ActivityId,
            cancellationToken);

        IReadOnlyCollection<ActivityDependencyResponse> response =
            items.Select(ActivityDependencyResponse.FromDomain).ToArray();

        return Result.Success(response);
    }
}
