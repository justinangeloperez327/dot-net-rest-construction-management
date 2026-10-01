using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.Dependencies;

public sealed class RemoveActivityDependencyCommandHandler(
    IActivityRepository activities,
    IActivityDependencyRepository dependencies,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<RemoveActivityDependencyCommand>
{
    public async Task<Result> HandleAsync(
        RemoveActivityDependencyCommand command,
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
            return Result.Failure(accessError);
        }

        var activity = await activities.GetAsync(
            command.ActivityId,
            cancellationToken);

        if (activity is null || activity.ProjectId != command.ProjectId)
        {
            return Result.Failure(
                ApplicationError.NotFound(
                    "Activities.NotFound",
                    "The activity was not found."));
        }

        var dependency = await dependencies.GetByIdAsync(
            command.DependencyId,
            cancellationToken);

        if (dependency is null || dependency.ActivityId != command.ActivityId)
        {
            return Result.Failure(
                ApplicationError.NotFound(
                    "Activities.DependencyNotFound",
                    "The activity dependency was not found."));
        }

        dependencies.Remove(dependency);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
