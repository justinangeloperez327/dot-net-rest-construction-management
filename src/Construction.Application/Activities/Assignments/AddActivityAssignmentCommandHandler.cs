using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.Assignments;

public sealed class AddActivityAssignmentCommandHandler(
    IActivityRepository activities,
    IProjectMemberRepository projectMembers,
    IActivityAssignmentRepository assignments,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<AddActivityAssignmentCommand, ActivityAssignmentResponse>
{
    public async Task<Result<ActivityAssignmentResponse>> HandleAsync(
        AddActivityAssignmentCommand command,
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
            return Result.Failure<ActivityAssignmentResponse>(accessError);
        }

        var activity = await activities.GetAsync(
            command.ActivityId,
            cancellationToken);

        if (activity is null || activity.ProjectId != command.ProjectId)
        {
            return Result.Failure<ActivityAssignmentResponse>(
                ApplicationError.NotFound(
                    "Activities.NotFound",
                    "The activity was not found."));
        }

        var projectMember = await projectMembers.GetAsync(
            command.ProjectId,
            command.UserId,
            cancellationToken);

        if (projectMember is null || !projectMember.IsActive)
        {
            return Result.Failure<ActivityAssignmentResponse>(
                ApplicationError.Validation(
                    "Activities.AssigneeNotProjectMember",
                    "The assigned user must be an active project member."));
        }

        if (await assignments.GetAsync(
            command.ActivityId,
            command.UserId,
            cancellationToken) is not null)
        {
            return Result.Failure<ActivityAssignmentResponse>(
                ApplicationError.Conflict(
                    "Activities.AssignmentAlreadyExists",
                    "The user is already assigned to the activity."));
        }

        ActivityAssignment assignment = ActivityAssignment.Create(
            command.ActivityId,
            command.UserId,
            command.Role);

        assignments.Add(assignment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(
            ActivityAssignmentResponse.FromDomain(assignment));
    }
}
