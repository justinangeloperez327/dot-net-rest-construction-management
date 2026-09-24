using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.Projects.GetProject;

public sealed class GetProjectQueryHandler(
    IProjectRepository projects,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetProjectQuery, ProjectResponse>
{
    public async Task<Result<ProjectResponse>> HandleAsync(
        GetProjectQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                query.ProjectId,
                Permissions.Projects.View,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ProjectResponse>(accessError);
        }

        var project = await projects.GetAsync(
            query.ProjectId,
            cancellationToken);

        return project is null
            ? Result.Failure<ProjectResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."))
            : Result.Success(ProjectResponse.FromDomain(project));
    }
}
