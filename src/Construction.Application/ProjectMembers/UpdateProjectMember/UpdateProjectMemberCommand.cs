using Construction.Application.Common.Messaging;
using Construction.Domain.ProjectMembers;

namespace Construction.Application.ProjectMembers.UpdateProjectMember;

public sealed record UpdateProjectMemberCommand(
    Guid ProjectId,
    Guid MemberId,
    ProjectMemberRole Role,
    bool IsActive) : ICommand<ProjectMemberResponse>;
