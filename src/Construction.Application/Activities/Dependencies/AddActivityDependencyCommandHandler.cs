using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.Dependencies;

public sealed class AddActivityDependencyCommandHandler(
    IActivityRepository activities,
    IActivityDependencyRepository dependencies,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<AddActivityDependencyCommand, ActivityDependencyResponse>
{
    public async Task<Result<ActivityDependencyResponse>> HandleAsync(
        AddActivityDependencyCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Activities.Manage,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ActivityDependencyResponse>(accessError);
        }

        var activity = await activities.GetAsync(
            command.ActivityId,
            cancellationToken);

        var predecessor = await activities.GetAsync(
            command.PredecessorActivityId,
            cancellationToken);

        if (activity is null || activity.ProjectId != command.ProjectId)
        {
            return Result.Failure<ActivityDependencyResponse>(
                ApplicationError.NotFound(
                    "Activities.NotFound",
                    "The activity was not found."));
        }

        if (predecessor is null || predecessor.ProjectId != command.ProjectId)
        {
            return Result.Failure<ActivityDependencyResponse>(
                ApplicationError.Validation(
                    "Activities.InvalidPredecessor",
                    "The predecessor activity must belong to the same project."));
        }

        if (await dependencies.GetAsync(
            command.ActivityId,
            command.PredecessorActivityId,
            cancellationToken) is not null)
        {
            return Result.Failure<ActivityDependencyResponse>(
                ApplicationError.Conflict(
                    "Activities.DependencyAlreadyExists",
                    "The activity dependency already exists."));
        }

        if (await dependencies.WouldCreateCycleAsync(
            command.ActivityId,
            command.PredecessorActivityId,
            cancellationToken))
        {
            return Result.Failure<ActivityDependencyResponse>(
                ApplicationError.Validation(
                    "Activities.DependencyCycleDetected",
                    "The dependency would create an activity cycle."));
        }

        ActivityDependency dependency = ActivityDependency.Create(
            command.ActivityId,
            command.PredecessorActivityId,
            command.Type,
            command.LagDays);

        dependencies.Add(dependency);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            ActivityDependencyResponse.FromDomain(dependency));
    }
}
