using Construction.Domain.Locations;

namespace Construction.Api.Contracts.Locations;

public sealed record UpdateProjectLocationRequest(
    string Name,
    LocationType Type,
    Guid? ParentLocationId,
    bool IsActive);
