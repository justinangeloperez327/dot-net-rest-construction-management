using Construction.Application.Common.Messaging;
using Construction.Domain.Locations;

namespace Construction.Application.Locations.CreateLocation;

public sealed record CreateLocationCommand(
    Guid ProjectId,
    string Name,
    LocationType Type,
    Guid? ParentLocationId) : ICommand<ProjectLocationResponse>;
