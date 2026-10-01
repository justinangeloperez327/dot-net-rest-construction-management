using Construction.Application.Common.Messaging;

namespace Construction.Application.Inspections.CancelInspection;

public sealed record CancelInspectionCommand(
    Guid ProjectId,
    Guid InspectionId,
    string? Reason) : ICommand<InspectionResponse>;
