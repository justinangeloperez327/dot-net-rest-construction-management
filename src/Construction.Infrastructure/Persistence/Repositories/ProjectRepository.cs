using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Projects;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository(ApplicationDbContext dbContext)
    : IProjectRepository
{
    public Task<Project?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects.SingleOrDefaultAsync(
            project => project.Id == id,
            cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.Projects.AnyAsync(
            project => project.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Project> Items, long TotalCount)> GetPageAsync(
        PageRequest page,
        Guid? userId = null,
        bool includeAllProjects = false,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<Project> query = dbContext.Projects.AsNoTracking();

        if (!includeAllProjects)
        {
            if (userId is not Guid memberUserId)
            {
                return (Array.Empty<Project>(), 0);
            }

            query = query.Where(project =>
                dbContext.ProjectMembers.Any(member =>
                    member.ProjectId == project.Id
                    && member.UserId == memberUserId
                    && member.IsActive));
        }

        query = query
            .OrderBy(project => project.Number);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        Project[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Project project)
    {
        ArgumentNullException.ThrowIfNull(project);
        dbContext.Projects.Add(project);
    }
}
