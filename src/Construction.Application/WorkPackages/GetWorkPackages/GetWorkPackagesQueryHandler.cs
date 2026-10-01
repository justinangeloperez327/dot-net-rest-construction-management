using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.WorkPackages.GetWorkPackages;

public sealed class GetWorkPackagesQueryHandler(
    IWorkPackageRepository workPackages,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetWorkPackagesQuery, IReadOnlyCollection<WorkPackageResponse>>
{
    public async Task<Result<IReadOnlyCollection<WorkPackageResponse>>> HandleAsync(
        GetWorkPackagesQuery query,
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
            return Result.Failure<IReadOnlyCollection<WorkPackageResponse>>(
                accessError);
        }

        var items = await workPackages.GetByProjectAsync(
            query.ProjectId,
            cancellationToken);

        IReadOnlyCollection<WorkPackageResponse> response =
            items.Select(WorkPackageResponse.FromDomain).ToArray();

        return Result.Success(response);
    }
}
