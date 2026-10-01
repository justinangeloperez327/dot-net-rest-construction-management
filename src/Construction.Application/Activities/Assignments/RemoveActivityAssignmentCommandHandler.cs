using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.Assignments;

public sealed class RemoveActivityAssignmentCommandHandler(
    IActivityRepository activities,
    IActivityAssignmentRepository assignments,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<RemoveActivityAssignmentCommand>
{
    public async Task<Result> HandleAsync(
        RemoveActivityAssignmentCommand command,
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

        var assignment = await assignments.GetByIdAsync(
            command.AssignmentId,
            cancellationToken);

        if (assignment is null || assignment.ActivityId != command.ActivityId)
        {
            return Result.Failure(
                ApplicationError.NotFound(
                    "Activities.AssignmentNotFound",
                    "The activity assignment was not found."));
        }

        assignments.Remove(assignment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
