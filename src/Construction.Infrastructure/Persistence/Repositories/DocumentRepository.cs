using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Documents;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class DocumentRepository(ApplicationDbContext dbContext)
    : IDocumentRepository
{
    public Task<ProjectDocument?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Documents
            .Include(document => document.Revisions)
            .SingleOrDefaultAsync(
                document => document.Id == id,
                cancellationToken);

    public Task<bool> ExistsByNumberAsync(
        Guid projectId,
        string normalizedNumber,
        CancellationToken cancellationToken = default) =>
        dbContext.Documents.AnyAsync(
            document =>
                document.ProjectId == projectId
                && document.NormalizedNumber == normalizedNumber,
            cancellationToken);

    public async Task<(IReadOnlyCollection<ProjectDocument> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(
            page.PageSize,
            1,
            PageRequest.MaximumPageSize);

        IQueryable<ProjectDocument> query = dbContext.Documents
            .AsNoTracking()
            .Where(document => document.ProjectId == projectId)
            .OrderBy(document => document.Number);

        long totalCount =
            await query.LongCountAsync(cancellationToken);

        ProjectDocument[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(ProjectDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
        dbContext.Documents.Add(document);
    }
}
