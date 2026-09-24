using Construction.Domain.ProjectMembers;

namespace Construction.Api.Contracts.ProjectMembers;

public sealed record UpdateProjectMemberRequest(
    ProjectMemberRole Role,
    bool IsActive);
