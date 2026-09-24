using Construction.Application.Common.Messaging;

namespace Construction.Application.Locations.GetLocations;

public sealed record GetLocationsQuery(Guid ProjectId)
    : IQuery<IReadOnlyCollection<ProjectLocationResponse>>;
