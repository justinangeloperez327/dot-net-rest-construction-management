using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.WorkPackages;

namespace Construction.Application.WorkPackages.ChangeWorkPackageStatus;

public sealed class ChangeWorkPackageStatusCommandHandler(
    IWorkPackageRepository workPackages,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<ChangeWorkPackageStatusCommand, WorkPackageResponse>
{
    public async Task<Result<WorkPackageResponse>> HandleAsync(
        ChangeWorkPackageStatusCommand command,
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

        switch (command.Status)
        {
            case WorkPackageStatus.Active:
                workPackage.Activate();
                break;
            case WorkPackageStatus.Completed:
                workPackage.Complete();
                break;
            case WorkPackageStatus.Archived:
                workPackage.Archive();
                break;
            default:
                return Result.Failure<WorkPackageResponse>(
                    ApplicationError.Validation(
                        "WorkPackages.InvalidStatusTransition",
                        "The requested work package status transition is not supported."));
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(WorkPackageResponse.FromDomain(workPackage));
    }
}
