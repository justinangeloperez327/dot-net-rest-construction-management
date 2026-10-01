using Construction.Application.Common.Pagination;
using Construction.Domain.Equipment;

namespace Construction.Application.Abstractions.Data;

public interface IEquipmentRepository
{
    Task<Equipment?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByAssetCodeAsync(
        Guid projectId,
        string normalizedAssetCode,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Equipment> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Equipment equipment);
}
