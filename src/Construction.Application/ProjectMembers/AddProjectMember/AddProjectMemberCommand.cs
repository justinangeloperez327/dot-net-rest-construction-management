using Construction.Application.Common.Messaging;
using Construction.Domain.ProjectMembers;

namespace Construction.Application.ProjectMembers.AddProjectMember;

public sealed record AddProjectMemberCommand(
    Guid ProjectId,
    Guid UserId,
    ProjectMemberRole Role) : ICommand<ProjectMemberResponse>;
