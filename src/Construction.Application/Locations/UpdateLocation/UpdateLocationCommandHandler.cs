using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Locations.UpdateLocation;

public sealed class UpdateLocationCommandHandler(
    IProjectLocationRepository locations,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateLocationCommand, ProjectLocationResponse>
{
    public async Task<Result<ProjectLocationResponse>> HandleAsync(
        UpdateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Projects.Update,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ProjectLocationResponse>(accessError);
        }

        var location = await locations.GetByIdAsync(
            command.LocationId,
            cancellationToken);

        if (location is null || location.ProjectId != command.ProjectId)
        {
            return Result.Failure<ProjectLocationResponse>(
                ApplicationError.NotFound(
                    "Locations.NotFound",
                    "The project location was not found."));
        }

        if (command.ParentLocationId is Guid parentId)
        {
            var parent = await locations.GetByIdAsync(
                parentId,
                cancellationToken);

            if (parent is null || parent.ProjectId != command.ProjectId)
            {
                return Result.Failure<ProjectLocationResponse>(
                    ApplicationError.Validation(
                        "Locations.InvalidParent",
                        "The parent location must belong to the same project."));
            }
        }

        string normalizedName = command.Name.Trim().ToUpperInvariant();

        if (await locations.ExistsByNameAsync(
            command.ProjectId,
            normalizedName,
            command.ParentLocationId,
            command.LocationId,
            cancellationToken))
        {
            return Result.Failure<ProjectLocationResponse>(
                ApplicationError.Conflict(
                    "Locations.NameAlreadyExists",
                    "A location with the same name already exists at this level."));
        }

        if (!command.IsActive)
        {
            location.Deactivate();
        }
        else
        {
            location.Update(
                command.Name,
                command.Type,
                command.ParentLocationId);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectLocationResponse.FromDomain(location));
    }
}
