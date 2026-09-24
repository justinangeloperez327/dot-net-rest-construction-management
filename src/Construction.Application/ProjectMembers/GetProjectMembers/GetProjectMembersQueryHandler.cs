using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.ProjectMembers.GetProjectMembers;

public sealed class GetProjectMembersQueryHandler(
    IProjectMemberRepository members,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : IQueryHandler<GetProjectMembersQuery, IReadOnlyCollection<ProjectMemberResponse>>
{
    public async Task<Result<IReadOnlyCollection<ProjectMemberResponse>>> HandleAsync(
        GetProjectMembersQuery query,
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
            return Result.Failure<IReadOnlyCollection<ProjectMemberResponse>>(
                accessError);
        }

        var projectMembers = await members.GetByProjectAsync(
            query.ProjectId,
            cancellationToken);

        IReadOnlyCollection<ProjectMemberResponse> response =
            projectMembers
                .Select(ProjectMemberResponse.FromDomain)
                .ToArray();

        return Result.Success(response);
    }
}
