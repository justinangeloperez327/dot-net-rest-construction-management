using Construction.Domain.Locations;

namespace Construction.Application.Locations;

public sealed record ProjectLocationResponse(
    Guid Id,
    Guid ProjectId,
    string Name,
    LocationType Type,
    Guid? ParentLocationId,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static ProjectLocationResponse FromDomain(ProjectLocation location)
    {
        ArgumentNullException.ThrowIfNull(location);

        return new ProjectLocationResponse(
            location.Id,
            location.ProjectId,
            location.Name,
            location.Type,
            location.ParentLocationId,
            location.IsActive,
            location.CreatedAtUtc,
            location.LastModifiedAtUtc);
    }
}
