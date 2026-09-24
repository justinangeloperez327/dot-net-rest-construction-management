using Construction.Domain.ProjectMembers;

namespace Construction.Application.ProjectMembers;

public sealed record ProjectMemberResponse(
    Guid Id,
    Guid ProjectId,
    Guid UserId,
    ProjectMemberRole Role,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static ProjectMemberResponse FromDomain(ProjectMember member)
    {
        ArgumentNullException.ThrowIfNull(member);

        return new ProjectMemberResponse(
            member.Id,
            member.ProjectId,
            member.UserId,
            member.Role,
            member.IsActive,
            member.CreatedAtUtc,
            member.LastModifiedAtUtc);
    }
}
