using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Locations.GetLocations;

public sealed class GetLocationsQueryHandler(
    IProjectLocationRepository locations,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetLocationsQuery, IReadOnlyCollection<ProjectLocationResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProjectLocationResponse>>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Projects.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<IReadOnlyCollection<ProjectLocationResponse>>(
                accessError);
        }

        var projectLocations = await locations.GetByProjectAsync(
            query.ProjectId,
            cancellationToken);

        IReadOnlyCollection<ProjectLocationResponse> response =
            projectLocations
                .Select(ProjectLocationResponse.FromDomain)
                .ToArray();

        return Result.Success(response);
    }
}
