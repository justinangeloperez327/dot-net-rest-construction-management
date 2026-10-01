using Construction.Application.Common.Messaging;

namespace Construction.Application.Inspections.PerformInspection;

public sealed record StartInspectionCommand(
    Guid ProjectId,
    Guid InspectionId) : ICommand<InspectionResponse>;
