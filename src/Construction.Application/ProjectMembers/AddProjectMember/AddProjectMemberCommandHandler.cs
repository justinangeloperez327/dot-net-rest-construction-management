using Construction.Application.Abstractions.Authentication;
using Construction.Application.Abstractions.Authorization;
using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Authorization;
using Construction.Application.Common.Errors;
using Construction.Application.Common.Messaging;
using Construction.Application.Common.Results;
using Construction.Domain.ProjectMembers;

namespace Construction.Application.ProjectMembers.AddProjectMember;

public sealed class AddProjectMemberCommandHandler(
    IProjectRepository projects,
    IProjectMemberRepository members,
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    IProjectAccessService projectAccessService)
    : ICommandHandler<AddProjectMemberCommand, ProjectMemberResponse>
{
    public async Task<Result<ProjectMemberResponse>> HandleAsync(
        AddProjectMemberCommand command,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

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

        if (await projects.GetAsync(command.ProjectId, cancellationToken) is null)
        {
            return Result.Failure<ProjectMemberResponse>(
                ApplicationError.NotFound(
                    "Projects.NotFound",
                    "The project was not found."));
        }

        var existing = await members.GetAsync(
            command.ProjectId,
            command.UserId,
            cancellationToken);

        if (existing is not null)
        {
            if (existing.IsActive)
            {
                return Result.Failure<ProjectMemberResponse>(
                    ApplicationError.Conflict(
                        "ProjectMembers.AlreadyExists",
                        "The user is already a member of the project."));
            }

            existing.Activate();
            existing.ChangeRole(command.Role);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success(ProjectMemberResponse.FromDomain(existing));
        }

        ProjectMember member = ProjectMember.Create(
            command.ProjectId,
            command.UserId,
            command.Role);

        members.Add(member);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(ProjectMemberResponse.FromDomain(member));
    }
}
