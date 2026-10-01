using Construction.Domain.Inspections;

namespace Construction.Api.Contracts.Inspections;

public sealed record UpdateInspectionRequest(
    string Title,
    string? Description,
    InspectionType Type,
    Guid? LocationId,
    Guid? ActivityId,
    DateOnly? RequestedForDate,
    Guid? InspectorUserId);
