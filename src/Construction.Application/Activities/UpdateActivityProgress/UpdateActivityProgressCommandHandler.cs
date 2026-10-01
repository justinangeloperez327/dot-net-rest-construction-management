using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.UpdateActivityProgress;

public sealed class UpdateActivityProgressCommandHandler(
    IActivityRepository activities,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateActivityProgressCommand, ActivityResponse>
{
    public async Task<Result<ActivityResponse>> HandleAsync(
        UpdateActivityProgressCommand command,
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
            return Result.Failure<ActivityResponse>(accessError);
        }

        var activity = await activities.GetAsync(
            command.ActivityId,
            cancellationToken);

        if (activity is null || activity.ProjectId != command.ProjectId)
        {
            return Result.Failure<ActivityResponse>(
                ApplicationError.NotFound(
                    "Activities.NotFound",
                    "The activity was not found."));
        }

        activity.UpdateProgress(command.ProgressPercentage);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ActivityResponse.FromDomain(activity));
    }
}
