using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.WorkPackages;

namespace Construction.Application.WorkPackages.UpdateWorkPackage;

public sealed class UpdateWorkPackageCommandHandler(
    IWorkPackageRepository workPackages,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateWorkPackageCommand, WorkPackageResponse>
{
    public async Task<Result<WorkPackageResponse>> HandleAsync(
        UpdateWorkPackageCommand command,
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

        WorkPackage? workPackage = await workPackages.GetAsync(
            command.WorkPackageId,
            cancellationToken);

        if (workPackage is null || workPackage.ProjectId != command.ProjectId)
        {
            return Result.Failure<WorkPackageResponse>(
                ApplicationError.NotFound(
                    "WorkPackages.NotFound",
                    "The work package was not found."));
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

            if (await workPackages.WouldCreateCycleAsync(
                command.WorkPackageId,
                parentId,
                cancellationToken))
            {
                return Result.Failure<WorkPackageResponse>(
                    ApplicationError.Validation(
                        "WorkPackages.CycleDetected",
                        "The requested parent would create a work package cycle."));
            }
        }

        workPackage.Update(
            command.Name,
            command.Description,
            command.ParentWorkPackageId);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(WorkPackageResponse.FromDomain(workPackage));
    }
}
