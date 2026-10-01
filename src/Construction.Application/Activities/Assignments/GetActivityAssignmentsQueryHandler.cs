using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.Assignments;

public sealed class GetActivityAssignmentsQueryHandler(
    IActivityRepository activities,
    IActivityAssignmentRepository assignments,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetActivityAssignmentsQuery, IReadOnlyCollection<ActivityAssignmentResponse>>
{
    public async Task<Result<IReadOnlyCollection<ActivityAssignmentResponse>>> HandleAsync(
        GetActivityAssignmentsQuery query,
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
            return Result.Failure<IReadOnlyCollection<ActivityAssignmentResponse>>(
                accessError);
        }

        var activity = await activities.GetAsync(
            query.ActivityId,
            cancellationToken);

        if (activity is null || activity.ProjectId != query.ProjectId)
        {
            return Result.Failure<IReadOnlyCollection<ActivityAssignmentResponse>>(
                ApplicationError.NotFound(
                    "Activities.NotFound",
                    "The activity was not found."));
        }

        var items = await assignments.GetByActivityAsync(
            query.ActivityId,
            cancellationToken);

        IReadOnlyCollection<ActivityAssignmentResponse> response =
            items.Select(ActivityAssignmentResponse.FromDomain).ToArray();

        return Result.Success(response);
    }
}
