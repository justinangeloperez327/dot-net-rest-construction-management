using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Locations;

namespace Construction.Application.Locations.CreateLocation;

public sealed class CreateLocationCommandHandler(
    IProjectLocationRepository locations,
    IProjectRepository projects,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CreateLocationCommand, ProjectLocationResponse>
{
    public async Task<Result<ProjectLocationResponse>> HandleAsync(
        CreateLocationCommand command,
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

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<ProjectLocationResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
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
            cancellationToken: cancellationToken))
        {
            return Result.Failure<ProjectLocationResponse>(
                ApplicationError.Conflict(
                    "Locations.NameAlreadyExists",
                    "A location with the same name already exists at this level."));
        }

        ProjectLocation location = ProjectLocation.Create(
            command.ProjectId,
            command.Name,
            command.Type,
            command.ParentLocationId);

        locations.Add(location);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectLocationResponse.FromDomain(location));
    }
}
