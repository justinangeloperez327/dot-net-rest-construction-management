using Construction.Application.Common.Messaging;

namespace Construction.Application.ProjectMembers.GetProjectMembers;

public sealed record GetProjectMembersQuery(Guid ProjectId)
    : IQuery<IReadOnlyCollection<ProjectMemberResponse>>;
