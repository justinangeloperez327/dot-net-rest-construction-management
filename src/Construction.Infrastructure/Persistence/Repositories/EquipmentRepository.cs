using Construction.Application.Abstractions.Data;
using Construction.Application.Common.Pagination;
using Construction.Domain.Equipment;
using Microsoft.EntityFrameworkCore;

namespace Construction.Infrastructure.Persistence.Repositories;

public sealed class EquipmentRepository(ApplicationDbContext dbContext)
    : IEquipmentRepository
{
    public Task<Equipment?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default) =>
        dbContext.Equipment
            .AsSplitQuery()
            .Include(equipment => equipment.Assignments)
            .Include(equipment => equipment.MaintenanceRecords)
            .SingleOrDefaultAsync(equipment => equipment.Id == id, cancellationToken);

    public Task<bool> ExistsByAssetCodeAsync(
        Guid projectId,
        string normalizedAssetCode,
        CancellationToken cancellationToken = default) =>
        dbContext.Equipment.AnyAsync(
            equipment =>
                equipment.ProjectId == projectId
                && equipment.NormalizedAssetCode == normalizedAssetCode,
            cancellationToken);

    public async Task<(IReadOnlyCollection<Equipment> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default)
    {
        int pageNumber = Math.Max(1, page.PageNumber);
        int pageSize = Math.Clamp(page.PageSize, 1, PageRequest.MaximumPageSize);

        IQueryable<Equipment> query = dbContext.Equipment
            .AsNoTracking()
            .Where(equipment => equipment.ProjectId == projectId)
            .OrderBy(equipment => equipment.AssetCode);

        long totalCount = await query.LongCountAsync(cancellationToken);
        Equipment[] items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(Equipment equipment) => dbContext.Equipment.Add(equipment);
}
