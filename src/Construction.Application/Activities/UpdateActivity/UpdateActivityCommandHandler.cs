using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Activities.UpdateActivity;

public sealed class UpdateActivityCommandHandler(
    IActivityRepository activities,
    IWorkPackageRepository workPackages,
    IProjectLocationRepository locations,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateActivityCommand, ActivityResponse>
{
    public async Task<Result<ActivityResponse>> HandleAsync(
        UpdateActivityCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

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

        if (command.WorkPackageId is Guid workPackageId)
        {
            var workPackage =
                await workPackages.GetAsync(workPackageId, cancellationToken);

            if (workPackage is null
                || workPackage.ProjectId != command.ProjectId)
            {
                return Result.Failure<ActivityResponse>(
                    ApplicationError.Validation(
                        "Activities.InvalidWorkPackage",
                        "The work package must belong to the same project."));
            }
        }

        if (command.LocationId is Guid locationId)
        {
            var location =
                await locations.GetByIdAsync(locationId, cancellationToken);

            if (location is null
                || location.ProjectId != command.ProjectId)
            {
                return Result.Failure<ActivityResponse>(
                    ApplicationError.Validation(
                        "Activities.InvalidLocation",
                        "The location must belong to the same project."));
            }
        }

        activity.Update(
            command.Name,
            command.Description,
            command.WorkPackageId,
            command.LocationId,
            command.Priority,
            command.PlannedStartDate,
            command.PlannedEndDate);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ActivityResponse.FromDomain(activity));
    }
}
