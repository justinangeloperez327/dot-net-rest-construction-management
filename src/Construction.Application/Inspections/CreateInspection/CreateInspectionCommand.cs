using Construction.Application.Common.Messaging;
using Construction.Domain.Inspections;

namespace Construction.Application.Inspections.CreateInspection;

public sealed record CreateInspectionCommand(
    Guid ProjectId,
    string Number,
    string Title,
    string? Description,
    InspectionType Type,
    Guid? LocationId,
    Guid? ActivityId,
    DateOnly? RequestedForDate,
    Guid? InspectorUserId) : ICommand<InspectionResponse>;
