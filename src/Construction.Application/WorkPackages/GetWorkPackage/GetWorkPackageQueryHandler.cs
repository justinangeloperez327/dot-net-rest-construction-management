using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.WorkPackages.GetWorkPackage;

public sealed class GetWorkPackageQueryHandler(
    IWorkPackageRepository workPackages,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetWorkPackageQuery, WorkPackageResponse>
{
    public async Task<Result<WorkPackageResponse>> HandleAsync(
        GetWorkPackageQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Activities.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<WorkPackageResponse>(accessError);
        }

        var workPackage = await workPackages.GetAsync(
            query.WorkPackageId,
            cancellationToken);

        return workPackage is null || workPackage.ProjectId != query.ProjectId
            ? Result.Failure<WorkPackageResponse>(
                ApplicationError.NotFound(
                    "WorkPackages.NotFound",
                    "The work package was not found."))
            : Result.Success(WorkPackageResponse.FromDomain(workPackage));
    }
}
