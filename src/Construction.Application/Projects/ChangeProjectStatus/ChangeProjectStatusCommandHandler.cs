using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.Projects;

namespace Construction.Application.Projects.ChangeProjectStatus;

public sealed class ChangeProjectStatusCommandHandler(
    IProjectRepository projects,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService,
    TimeProvider timeProvider)
    : ICommandHandler<ChangeProjectStatusCommand, ProjectResponse>
{
    public async Task<Result<ProjectResponse>> HandleAsync(
        ChangeProjectStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        string permission = command.Status == ProjectStatus.Archived
            ? Permissions.Projects.Archive
            : Permissions.Projects.Update;

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                permission,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ProjectResponse>(accessError);
        }

        var project = await projects.GetAsync(
            command.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result.Failure<ProjectResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        switch (command.Status)
        {
            case ProjectStatus.Active:
                project.Start();
                break;
            case ProjectStatus.OnHold:
                project.PutOnHold();
                break;
            case ProjectStatus.Completed:
                project.Complete(
                    command.CompletionDate
                    ?? DateOnly.FromDateTime(
                        timeProvider.GetUtcNow().UtcDateTime));
                break;
            case ProjectStatus.Archived:
                project.Archive();
                break;
            default:
                return Result.Failure<ProjectResponse>(
                    ApplicationError.Validation(
                        "Projects.InvalidStatusTransition",
                        "The requested project status transition is not supported."));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectResponse.FromDomain(project));
    }
}
