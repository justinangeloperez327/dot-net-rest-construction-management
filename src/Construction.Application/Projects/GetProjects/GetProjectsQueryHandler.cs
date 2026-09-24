using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Pagination;
using Construction.Application.Common.Results;

namespace Construction.Application.Projects.GetProjects;

public sealed class GetProjectsQueryHandler(
    IProjectRepository projects,
    IPermissionService permissionService,
    ICurrentUser currentUser)
    : IQueryHandler<GetProjectsQuery, PagedResult<ProjectResponse>>
{
    public async Task<Result<PagedResult<ProjectResponse>>> HandleAsync(
        GetProjectsQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (currentUser.UserId is not Guid userId)
        {
            return Result.Failure<PagedResult<ProjectResponse>>(
                ApplicationError.Unauthorized(
                    "Authentication.Required",
                    "Authentication is required."));
        }

        IReadOnlySet<string> permissions =
            await permissionService.GetPermissionsAsync(
                userId,
                cancellationToken);

        if (!permissions.Contains(Permissions.Projects.View))
        {
            return Result.Failure<PagedResult<ProjectResponse>>(
                ApplicationError.Forbidden(
                    "Authorization.PermissionDenied",
                    "You do not have permission to view projects."));
        }

        bool includeAll =
            permissions.Contains(Permissions.Projects.AccessAll);

        var page = await projects.GetPageAsync(
            query.Page,
            userId,
            includeAll,
            cancellationToken);

        var response = new PagedResult<ProjectResponse>(
            page.Items.Select(ProjectResponse.FromDomain).ToArray(),
            Math.Max(1, query.Page.PageNumber),
            Math.Clamp(
                query.Page.PageSize,
                1,
                PageRequest.MaximumPageSize),
            page.TotalCount);

        return Result.Success(response);
    }
}
