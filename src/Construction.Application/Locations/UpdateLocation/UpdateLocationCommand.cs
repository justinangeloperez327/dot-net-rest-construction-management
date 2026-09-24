using Construction.Application.Common.Messaging;
using Construction.Domain.Locations;

namespace Construction.Application.Locations.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid ProjectId,
    Guid LocationId,
    string Name,
    LocationType Type,
    Guid? ParentLocationId,
    bool IsActive) : ICommand<ProjectLocationResponse>;
