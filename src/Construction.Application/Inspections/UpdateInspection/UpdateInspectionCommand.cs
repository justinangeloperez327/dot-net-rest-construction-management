using Construction.Application.Common.Messaging;
using Construction.Domain.Inspections;

namespace Construction.Application.Inspections.UpdateInspection;

public sealed record UpdateInspectionCommand(
    Guid ProjectId,
    Guid InspectionId,
    string Title,
    string? Description,
    InspectionType Type,
    Guid? LocationId,
    Guid? ActivityId,
    DateOnly? RequestedForDate,
    Guid? InspectorUserId) : ICommand<InspectionResponse>;
