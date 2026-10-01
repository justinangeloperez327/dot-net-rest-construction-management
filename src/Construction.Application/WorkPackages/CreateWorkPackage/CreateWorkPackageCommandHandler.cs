using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.WorkPackages;

namespace Construction.Application.WorkPackages.CreateWorkPackage;

public sealed class CreateWorkPackageCommandHandler(
    IProjectRepository projects,
    IWorkPackageRepository workPackages,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<CreateWorkPackageCommand, WorkPackageResponse>
{
    public async Task<Result<WorkPackageResponse>> HandleAsync(
        CreateWorkPackageCommand command,
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
            return Result.Failure<WorkPackageResponse>(accessError);
        }

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<WorkPackageResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        if (command.ParentWorkPackageId is Guid parentId)
        {
            WorkPackage? parent =
                await workPackages.GetAsync(parentId, cancellationToken);

            if (parent is null || parent.ProjectId != command.ProjectId)
            {
                return Result.Failure<WorkPackageResponse>(
                    ApplicationError.Validation(
                        "WorkPackages.InvalidParent",
                        "The parent work package must belong to the same project."));
            }
        }

        string normalizedCode = command.Code.Trim().ToUpperInvariant();

        if (await workPackages.ExistsByCodeAsync(
            command.ProjectId,
            normalizedCode,
            cancellationToken))
        {
            return Result.Failure<WorkPackageResponse>(
                ApplicationError.Conflict(
                    "WorkPackages.CodeAlreadyExists",
                    "A work package with the same code already exists in this project."));
        }

        WorkPackage workPackage = WorkPackage.Create(
            command.ProjectId,
            command.Code,
            command.Name,
            command.Description,
            command.ParentWorkPackageId);

        workPackages.Add(workPackage);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(WorkPackageResponse.FromDomain(workPackage));
    }
}
