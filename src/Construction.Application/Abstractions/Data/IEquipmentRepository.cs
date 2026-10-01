using Construction.Application.Common.Pagination;

namespace Construction.Application.Abstractions.Data;

public interface IEquipmentRepository
{
    Task<Construction.Domain.Equipment.Equipment?> GetAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByAssetCodeAsync(
        Guid projectId,
        string normalizedAssetCode,
        CancellationToken cancellationToken = default);

    Task<(IReadOnlyCollection<Construction.Domain.Equipment.Equipment> Items, long TotalCount)> GetPageAsync(
        Guid projectId,
        PageRequest page,
        CancellationToken cancellationToken = default);

    void Add(Construction.Domain.Equipment.Equipment equipment);
}
