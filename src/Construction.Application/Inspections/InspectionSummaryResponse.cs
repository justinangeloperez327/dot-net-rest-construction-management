using Construction.Domain.Inspections;

namespace Construction.Application.Inspections;

public sealed record InspectionSummaryResponse(
    Guid Id,
    string Number,
    string Title,
    InspectionType Type,
    InspectionStatus Status,
    Guid? LocationId,
    Guid? ActivityId,
    DateOnly? RequestedForDate,
    Guid? InspectorUserId,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static InspectionSummaryResponse FromDomain(Inspection inspection) =>
        new(
            inspection.Id,
            inspection.Number,
            inspection.Title,
            inspection.Type,
            inspection.Status,
            inspection.LocationId,
            inspection.ActivityId,
            inspection.RequestedForDate,
            inspection.InspectorUserId,
            inspection.CreatedAtUtc,
            inspection.LastModifiedAtUtc);
}
