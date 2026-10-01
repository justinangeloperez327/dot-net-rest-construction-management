using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.ChangeActivityStatus;

public sealed class ChangeActivityStatusCommandHandler(
    IActivityRepository activities,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ChangeActivityStatusCommand, ActivityResponse>
{
    public async Task<Result<ActivityResponse>> HandleAsync(
        ChangeActivityStatusCommand command,
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

        DateOnly effectiveDate =
            command.EffectiveDate
            ?? DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);

        switch (command.Status)
        {
            case ActivityStatus.InProgress:
                activity.Start(effectiveDate);
                break;
            case ActivityStatus.OnHold:
                activity.PutOnHold();
                break;
            case ActivityStatus.Completed:
                activity.Complete(effectiveDate);
                break;
            case ActivityStatus.Cancelled:
                activity.Cancel();
                break;
            default:
                return Result.Failure<ActivityResponse>(
                    ApplicationError.Validation(
                        "Activities.InvalidStatusTransition",
                        "The requested activity status transition is not supported."));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ActivityResponse.FromDomain(activity));
    }
}
