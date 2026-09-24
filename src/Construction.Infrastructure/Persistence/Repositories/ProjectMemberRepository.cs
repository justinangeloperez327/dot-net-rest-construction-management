using Construction.Application.Abstractions.Data;
using Construction.Domain.ProjectMembers;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class ProjectMemberRepository(ApplicationDbContext dbContext)
    : IProjectMemberRepository
{
    public Task<ProjectMember?> GetAsync(
        Guid projectId,
        Guid userId,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectMembers.SingleOrDefaultAsync(
            member =>
                member.ProjectId == projectId
                && member.UserId == userId,
            cancellationToken);

    public Task<ProjectMember?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.ProjectMembers.SingleOrDefaultAsync(
            member => member.Id == id,
            cancellationToken);

    public async Task<IReadOnlyCollection<ProjectMember>> GetByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await dbContext.ProjectMembers
            .AsNoTracking()
            .Where(member => member.ProjectId == projectId)
            .OrderByDescending(member => member.IsActive)
            .ThenBy(member => member.Role)
            .ToArrayAsync(cancellationToken);

    public void Add(ProjectMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        dbContext.ProjectMembers.Add(member);
    }
}
