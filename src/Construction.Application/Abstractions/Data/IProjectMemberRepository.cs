using Construction.Domain.ProjectMembers;

namespace Construction.Application.Abstractions.Data;

public interface IProjectMemberRepository
{
    Task<ProjectMember?> GetAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ProjectMember?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<ProjectMember>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    void Add(ProjectMember member);
}
