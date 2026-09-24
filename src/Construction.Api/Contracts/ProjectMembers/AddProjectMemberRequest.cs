using Construction.Domain.ProjectMembers;

namespace Construction.Api.Contracts.ProjectMembers;

public sealed record AddProjectMemberRequest(
    Guid UserId,
    ProjectMemberRole Role);
