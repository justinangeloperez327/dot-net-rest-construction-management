using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;

namespace Construction.Application.ProjectMembers.UpdateProjectMember;

public sealed class UpdateProjectMemberCommandHandler(
    IProjectMemberRepository members,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<UpdateProjectMemberCommand, ProjectMemberResponse>
{
    public async Task<Result<ProjectMemberResponse>> HandleAsync(
        UpdateProjectMemberCommand command,
        CancellationToken cancellationToken = default)
    {
        ApplicationError? accessError =
            await ProjectAccessGuard.CheckAsync(
                currentUser,
                projectAccessService,
                command.ProjectId,
                Permissions.Projects.ManageMembers,
                cancellationToken);

        if (accessError is not null)
        {
            return Result.Failure<ProjectMemberResponse>(accessError);
        }

        var member = await members.GetByIdAsync(
            command.MemberId,
            cancellationToken);

        if (member is null || member.ProjectId != command.ProjectId)
        {
            return Result.Failure<ProjectMemberResponse>(
                ApplicationError.NotFound(
                    "ProjectMembers.NotFound",
                    "The project member was not found."));
        }

        if (command.IsActive)
        {
            member.Activate();
            member.ChangeRole(command.Role);
        }
        else
        {
            member.Deactivate();
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectMemberResponse.FromDomain(member));
    }
}
