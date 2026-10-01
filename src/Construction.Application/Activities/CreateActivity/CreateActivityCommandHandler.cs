using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Activities;

namespace Construction.Application.Activities.CreateActivity;

public sealed class CreateActivityCommandHandler(
    IProjectRepository projects,
    IWorkPackageRepository workPackages,
    IProjectLocationRepository locations,
    IActivityRepository activities,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CreateActivityCommand, ActivityResponse>
{
    public async Task<Result<ActivityResponse>> HandleAsync(
        CreateActivityCommand command,
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

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<ActivityResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        ApplicationError? referenceError =
            await ValidateReferencesAsync(command, cancellationToken);

        if (referenceError is not null)
        {
            return Result.Failure<ActivityResponse>(referenceError);
        }

        string normalizedCode = command.Code.Trim().ToUpperInvariant();

        if (await activities.ExistsByCodeAsync(
            command.ProjectId,
            normalizedCode,
            cancellationToken))
        {
            return Result.Failure<ActivityResponse>(
                ApplicationError.Conflict(
                    "Activities.CodeAlreadyExists",
                    "An activity with the same code already exists in this project."));
        }

        Activity activity = Activity.Create(
            command.ProjectId,
            command.Code,
            command.Name,
            command.Description,
            command.WorkPackageId,
            command.LocationId,
            command.Priority,
            command.PlannedStartDate,
            command.PlannedEndDate);

        activities.Add(activity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ActivityResponse.FromDomain(activity));
    }

    private async Task<ApplicationError?> ValidateReferencesAsync(
        CreateActivityCommand command,
        CancellationToken cancellationToken)
    {
        if (command.WorkPackageId is Guid workPackageId)
        {
            var workPackage =
                await workPackages.GetAsync(workPackageId, cancellationToken);

            if (workPackage is null
                || workPackage.ProjectId != command.ProjectId)
            {
                return ApplicationError.Validation(
                    "Activities.InvalidWorkPackage",
                    "The work package must belong to the same project.");
            }
        }

        if (command.LocationId is Guid locationId)
        {
            var location =
                await locations.GetByIdAsync(locationId, cancellationToken);

            if (location is null
                || location.ProjectId != command.ProjectId)
            {
                return ApplicationError.Validation(
                    "Activities.InvalidLocation",
                    "The location must belong to the same project.");
            }
        }

        return null;
    }
}
