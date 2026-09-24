using Construction.Domain.Common;

namespace Construction.Domain.ProjectMembers;

public sealed class ProjectMember : AuditableEntity<Guid>
{
    private ProjectMember()
        : base(Guid.Empty)
    {
    }

    private ProjectMember(
        Guid id,
        Guid projectId,
        Guid userId,
        ProjectMemberRole role)
        : base(id)
    {
        ProjectId = projectId;
        UserId = userId;
        Role = role;
        IsActive = true;
    }

    public Guid ProjectId { get; private set; }

    public Guid UserId { get; private set; }

    public ProjectMemberRole Role { get; private set; }

    public bool IsActive { get; private set; }

    public static ProjectMember Create(
        Guid projectId,
        Guid userId,
        ProjectMemberRole role)
    {
        if (projectId == Guid.Empty)
        {
            throw new DomainException(
                "Project identifier is required.");
        }

        if (userId == Guid.Empty)
        {
            throw new DomainException(
                "User identifier is required.");
        }

        return new ProjectMember(
            Guid.CreateVersion7(),
            projectId,
            userId,
            role);
    }

    public void ChangeRole(ProjectMemberRole role)
    {
        if (!IsActive)
        {
            throw new DomainException(
                "Inactive project members cannot be updated.");
        }

        Role = role;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
