using Construction.Domain.Inspections;

namespace Construction.Application.Inspections;

public sealed record InspectionResponse(
    Guid Id,
    Guid ProjectId,
    string Number,
    string Title,
    string? Description,
    InspectionType Type,
    Guid? LocationId,
    Guid? ActivityId,
    DateOnly? RequestedForDate,
    Guid? InspectorUserId,
    Guid RequestedByUserId,
    InspectionStatus Status,
    string? ResultNotes,
    Guid? InspectedByUserId,
    DateTimeOffset? InspectedAtUtc,
    IReadOnlyCollection<InspectionHistoryResponse> History,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastModifiedAtUtc)
{
    public static InspectionResponse FromDomain(Inspection inspection)
    {
        ArgumentNullException.ThrowIfNull(inspection);

        return new InspectionResponse(
            inspection.Id,
            inspection.ProjectId,
            inspection.Number,
            inspection.Title,
            inspection.Description,
            inspection.Type,
            inspection.LocationId,
            inspection.ActivityId,
            inspection.RequestedForDate,
            inspection.InspectorUserId,
            inspection.RequestedByUserId,
            inspection.Status,
            inspection.ResultNotes,
            inspection.InspectedByUserId,
            inspection.InspectedAtUtc,
            inspection.History
                .OrderBy(entry => entry.OccurredAtUtc)
                .Select(InspectionHistoryResponse.FromDomain)
                .ToArray(),
            inspection.CreatedAtUtc,
            inspection.LastModifiedAtUtc);
    }
}

public sealed record InspectionHistoryResponse(
    Guid Id,
    InspectionHistoryAction Action,
    Guid ActorUserId,
    DateTimeOffset OccurredAtUtc,
    string? Note)
{
    public static InspectionHistoryResponse FromDomain(
        InspectionHistoryEntry entry) =>
        new(
            entry.Id,
            entry.Action,
            entry.ActorUserId,
            entry.OccurredAtUtc,
            entry.Note);
}
